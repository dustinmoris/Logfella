using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Logfella.Extensions;
using Microsoft.AspNetCore.Http;

namespace Logfella.LogWriters
{
    public sealed class GoogleCloudLogWriter : LogWriter
    {
        public class HttpRequest
        {
            public HttpRequest(HttpContext ctx)
            {
                var req = ctx.Request;

                RequestMethod = req.Method.ToUpper();
                RequestUrl = ctx.RequestUrl();
                RequestSize =
                    req.ContentLength.HasValue
                        ? req.ContentLength.Value.ToString()
                        : "0";
                UserAgent =
                    req.Headers.ContainsKey("User-Agent")
                        ? req.Headers["User-Agent"].ToString()
                        : "";
                RemoteIp = ctx.Connection.RemoteIpAddress.ToString();
                ServerIp = ctx.Connection.LocalIpAddress.ToString();
                Referer =
                    req.Headers.ContainsKey("Referer")
                        ? req.Headers["Referer"].ToString()
                        : "";
                Protocol = req.Protocol;
            }

            public string RequestMethod { get; }
            public string RequestUrl { get; }
            public string RequestSize { get; }
            public string UserAgent { get; }
            public string RemoteIp { get; }
            public string ServerIp { get; }
            public string Referer { get; }
            public string Protocol { get; }
        }

        public class ErrorContext
        {
            public ErrorContext(Exception ex)
            {
                ExceptionType = ex.GetType().ToString();
                ExceptionMessage = ex.Message;
                StackTrace = ex.StackTrace;
                InnerError =
                    ex.InnerException != null
                        ? new ErrorContext(ex.InnerException)
                        : null;
            }

            public string ExceptionType { get; }
            public string ExceptionMessage { get; }
            public string StackTrace { get; }
            public ErrorContext InnerError { get; }
        }

        private readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions
            {
                WriteIndented = false,
                AllowTrailingCommas = false,
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = false,
                PropertyNameCaseInsensitive = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

        private readonly string _serviceName;
        private readonly string _serviceVersion;
        private readonly bool _useGoogleCloudTimestamp;
        private readonly Dictionary<string, string> _labels;
        private HttpRequest _httpRequest;

        /// <summary>
        /// Initialises a new GoogleCloudLogWriter object.
        /// </summary>
        /// <param name="minSeverity">Minimum severity for the logger to write logs.</param>
        /// <param name="serviceName">Name of the application/service writing logs.</param>
        /// <param name="serviceVersion">Version of the application/version writing logs.</param>
        /// <param name="useGoogleCloudTimestamp">If set to true, then Google Cloud Logging will set its own timestamp for log entries when they have been written to stdout.</param>
        /// <param name="labels">Optional labels to attach to all log entries.</param>
        /// <param name="correlationIdKey">The key under which a correlationId will be logged.</param>
        /// <param name="correlationId">An ID to correlate a set of logs (e.g. per HTTP request).</param>
        public GoogleCloudLogWriter(
            Severity minSeverity,
            string serviceName = null,
            string serviceVersion = null,
            bool useGoogleCloudTimestamp = false,
            IDictionary<string, string> labels = null,
            string correlationIdKey = "correlationId",
            string correlationId = "")
            : base(
                minSeverity,
                correlationIdKey,
                correlationId)
        {
            _serviceName = serviceName;
            _serviceVersion = serviceVersion;
            _useGoogleCloudTimestamp = useGoogleCloudTimestamp;

            if (labels != null)
                _labels = new Dictionary<string, string>(labels);
        }

        /// <summary>
        /// If true then the JSON log output will be written in indented format for better human readability.
        /// <para>Use this setting to debug log output during development.</para>
        /// </summary>
        public bool PrettifyOutput
        {
            set => _jsonOptions.WriteIndented = value;
        }

        public ILogWriter IncludeHttpRequest(HttpContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            _httpRequest = new HttpRequest(ctx);

            return this;
        }

        // In the GCP any logs written directly to stdout will be picked up by
        // Google StackDriver Logging and indexed correctly.
        // Structured logs are supported by writing a JSON payload into stdout:
        // https://cloud.google.com/logging/docs/structured-logging
        //
        // Special fields which StackDriver will identify automatically:
        // https://cloud.google.com/logging/docs/agent/configuration#special-fields
        protected override void WriteLog(
            Severity severity,
            string message,
            IDictionary<string, object> data,
            Exception ex)
        {
            var msg = new StringBuilder();
            msg.Append(message);

            var logEntry = new Dictionary<string, object>
            {
                {"message", msg.ToString()},
                {"severity", severity.String()}
            };

            if (!_useGoogleCloudTimestamp)
                logEntry.Add("time", DateTime.UtcNow.ToString("o"));

            if (string.IsNullOrEmpty(_serviceName))
                logEntry.Add("serviceContext.service", _serviceName);

            if (string.IsNullOrEmpty(_serviceVersion))
                logEntry.Add("serviceContext.version", _serviceVersion);

            if (ex != null)
            {
                logEntry.Add("@type", "type.googleapis.com/google.devtools.clouderrorreporting.v1beta1.ReportedErrorEvent");
                logEntry.Add("error", new ErrorContext(ex));
            }

            if (_httpRequest != null)
                logEntry.Add("httpRequest", _httpRequest);

            if (_labels != null && _labels.Count > 0)
                logEntry.Add("logging.googleapis.com/labels", _labels);

            if (data != null && data.Keys.Count > 0)
                logEntry.Add("data", data as Dictionary<string, object>);

            var json = JsonSerializer.Serialize(logEntry, _jsonOptions);

            Console.WriteLine(json);
        }
    }
}