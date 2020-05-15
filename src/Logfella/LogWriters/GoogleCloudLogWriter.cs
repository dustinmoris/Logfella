using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Logfella.LogWriters
{
    public sealed class GoogleCloudLogWriter : LogWriter
    {
        // ------------------------------------------
        // Sub types to serialise extra log data
        // ------------------------------------------

        public class HttpRequestContext
        {
            public HttpRequestContext(HttpContext ctx)
            {
                var req = ctx.Request;
                var path = req.PathBase.HasValue || req.Path.HasValue
                    ? req.PathBase.Value + req.Path.Value
                    : "/";
                var requestUrl =
                    $"{req.Scheme}://{req.Host.Value}{path}{req.QueryString.Value}";

                RequestMethod = req.Method.ToUpper();
                RequestUrl = requestUrl;
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

            private HttpRequestContext(
                string requestMethod,
                string requestUrl,
                string requestSize,
                string userAgent,
                string remoteIp,
                string serverIp,
                string referer,
                string protocol)
            {
                RequestMethod = requestMethod;
                RequestUrl = requestUrl;
                RequestSize = requestSize;
                UserAgent = userAgent;
                RemoteIp = remoteIp;
                ServerIp = serverIp;
                Referer = referer;
                Protocol = protocol;
            }

            public HttpRequestContext DeepClone() =>
                new HttpRequestContext(
                    RequestMethod,
                    RequestUrl,
                    RequestSize,
                    UserAgent,
                    RemoteIp,
                    ServerIp,
                    Referer,
                    Protocol);

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

        // ------------------------------------------
        // JSON Serializer settings
        // ------------------------------------------

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

        // ------------------------------------------
        // Google Cloud LogEntry Keys & Values
        // ------------------------------------------

        private static class Keys
        {
            // Property names which the GCP log analyzer will
            // automatically pick up from the jsonPayload:
            // (See: https://cloud.google.com/logging/docs/agent/configuration#special-fields)

            public const string Severity = "severity";
            public const string Time = "time";
            public const string ServiceName = "serviceContext.service";
            public const string ServiceVersion = "serviceContext.version";
            public const string Message = "message";
            public const string Type = "@type";
            public const string HttpRequest = "httpRequest";
            public const string Labels = "logging.googleapis.com/labels";
        }

        private static class Values
        {
            public const string ErrorType =
                "type.googleapis.com/google.devtools.clouderrorreporting.v1beta1.ReportedErrorEvent";
        }

        // ------------------------------------------
        // Private fields
        // ------------------------------------------

        private const string DefaultCorrelationIdKey = "correlationId";
        private readonly string _serviceName;
        private readonly string _serviceVersion;
        private readonly bool _useGoogleCloudTimestamp;
        private readonly Dictionary<string, string> _labels;
        private readonly HttpRequestContext _httpRequestContext;

        // ------------------------------------------
        // Private static helper methods for immutability
        // ------------------------------------------

        private static Dictionary<string, string> Clone(IDictionary<string, string> labels) =>
            labels.Keys.ToDictionary(
                key => key,
                key => labels[key]);

        private static Dictionary<string, string> CloneAndAdd(
            IDictionary<string, string> labels,
            params (string key, string value)[] additionalLabels)
        {
            var clonedLabels = Clone(labels);
            foreach (var (key, value) in additionalLabels)
                clonedLabels.Add(key, value);
            return clonedLabels;
        }

        // ------------------------------------------
        // Main public constructor
        // ------------------------------------------

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
        /// <param name="ctx">Additional HTTP data to be set for a per-request created logger.</param>
        public GoogleCloudLogWriter(
            Severity minSeverity,
            string serviceName = null,
            string serviceVersion = null,
            bool useGoogleCloudTimestamp = false,
            IDictionary<string, string> labels = null,
            string correlationIdKey = DefaultCorrelationIdKey,
            string correlationId = null,
            HttpContext ctx = null)
            : base(
                minSeverity,
                correlationIdKey,
                correlationId)
        {
            _serviceName = serviceName;
            _serviceVersion = serviceVersion;
            _useGoogleCloudTimestamp = useGoogleCloudTimestamp;

            if (labels != null)
                _labels = Clone(labels);

            if (ctx != null)
                _httpRequestContext = new HttpRequestContext(ctx);
        }

        // ------------------------------------------
        // Private constructor and public Clone method
        // ------------------------------------------

        private GoogleCloudLogWriter(
            Severity minSeverity,
            string serviceName,
            string serviceVersion,
            bool useGoogleCloudTimestamp,
            IDictionary<string, string> labels,
            string correlationIdKey,
            string correlationId,
            HttpRequestContext requestCtx)
            : this(
                minSeverity,
                serviceName,
                serviceVersion,
                useGoogleCloudTimestamp,
                labels,
                correlationIdKey,
                correlationId)
        {
            _httpRequestContext = requestCtx.DeepClone();
        }

        public GoogleCloudLogWriter DeepClone() =>
            new GoogleCloudLogWriter(
                MinSeverity,
                _serviceName,
                _serviceVersion,
                _useGoogleCloudTimestamp,
                // Labels will be deep cloned in the ctor:
                _labels,
                CorrelationIdKey,
                CorrelationId,
                // The requestContext will be cloned in the ctor:
                _httpRequestContext);

        // ------------------------------------------
        // Private constructor and public builder methods
        // ------------------------------------------

        public static GoogleCloudLogWriter Create(Severity minSeverity) =>
            new GoogleCloudLogWriter(minSeverity);

        public GoogleCloudLogWriter AddServiceContext(string serviceName, string serviceVersion) =>
            new GoogleCloudLogWriter(
                MinSeverity,
                serviceName,
                serviceVersion,
                _useGoogleCloudTimestamp,
                _labels,
                CorrelationIdKey,
                CorrelationId,
                _httpRequestContext);

        public GoogleCloudLogWriter UseGoogleCloudTimestamp() =>
            new GoogleCloudLogWriter(
                MinSeverity,
                _serviceName,
                _serviceVersion,
                true,
                _labels,
                CorrelationIdKey,
                CorrelationId,
                _httpRequestContext);

        public GoogleCloudLogWriter AddLabels(IDictionary<string, string> labels) =>
            new GoogleCloudLogWriter(
                MinSeverity,
                _serviceName,
                _serviceVersion,
                _useGoogleCloudTimestamp,
                labels,
                CorrelationIdKey,
                CorrelationId,
                _httpRequestContext);

        public GoogleCloudLogWriter AddLabel(string key, string value) =>
            new GoogleCloudLogWriter(
                MinSeverity,
                _serviceName,
                _serviceVersion,
                _useGoogleCloudTimestamp,
                CloneAndAdd(_labels, (key, value)),
                CorrelationIdKey,
                CorrelationId,
                _httpRequestContext);

        public GoogleCloudLogWriter AddCorrelationId(
            string correlationId,
            string key = DefaultCorrelationIdKey) =>
            new GoogleCloudLogWriter(
                MinSeverity,
                _serviceName,
                _serviceVersion,
                _useGoogleCloudTimestamp,
                _labels,
                key,
                correlationId,
                _httpRequestContext);

        public GoogleCloudLogWriter AddHttpContext(HttpContext httpContext) =>
            new GoogleCloudLogWriter(
                MinSeverity,
                _serviceName,
                _serviceVersion,
                _useGoogleCloudTimestamp,
                _labels,
                CorrelationIdKey,
                CorrelationId,
                httpContext);

        /// <summary>
        /// Use this setting to debug log output during development.
        /// </summary>
        public GoogleCloudLogWriter PrettifyOutputForDebugging()
        {
            var instance =
                new GoogleCloudLogWriter(
                    MinSeverity,
                    _serviceName,
                    _serviceVersion,
                    _useGoogleCloudTimestamp,
                    _labels,
                    CorrelationIdKey,
                    CorrelationId,
                    _httpRequestContext);
            instance._jsonOptions.WriteIndented = true;
            return instance;
        }

        // ------------------------------------------
        // Main logging method
        // ------------------------------------------

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
            var logEntry = new Dictionary<string, object>
                {
                    {Keys.Severity, severity.String()}
                };

            if (!_useGoogleCloudTimestamp)
                logEntry.Add(Keys.Time, DateTime.UtcNow.ToString("o"));

            if (string.IsNullOrEmpty(_serviceName))
                logEntry.Add(Keys.ServiceName, _serviceName);

            if (string.IsNullOrEmpty(_serviceVersion))
                logEntry.Add(Keys.ServiceVersion, _serviceVersion);

            if (ex != null)
            {
                logEntry.Add(Keys.Message, $"{message}\n\nException:\n\n{ex}");
                //logEntry.Add(Keys.Type, Values.ErrorType);
                logEntry.Add("error", new ErrorContext(ex));
            }
            else
            {
                logEntry.Add(Keys.Message, message);
            }

            if (_httpRequestContext != null)
                logEntry.Add(Keys.HttpRequest, _httpRequestContext);

            if (_labels != null && _labels.Count > 0)
                logEntry.Add(Keys.Labels, _labels);

            if (data != null && data.Keys.Count > 0)
                logEntry.Add("data", data as Dictionary<string, object>);

            var json = JsonSerializer.Serialize(logEntry, _jsonOptions);

            Console.WriteLine(json);
        }
    }
}