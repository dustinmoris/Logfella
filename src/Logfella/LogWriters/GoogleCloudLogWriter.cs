using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Logfella.LogWriters
{
    public class GoogleCloudLogWriter : LogWriter
    {
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

        /// <summary>
        /// Initialises a new GoogleCloudLogWriter object.
        /// </summary>
        /// <param name="minSeverity">Minimum severity for the logger to write logs.</param>
        /// <param name="serviceName">Name of the application/service writing logs.</param>
        /// <param name="serviceVersion">Version of the application/version writing logs.</param>
        /// <param name="useGoogleCloudTimestamp">If set to true, then Google Cloud Logging will set its own timestamp for log entries when they have been written to stdout.</param>
        /// <param name="labels">Optional labels to attach to all log entries.</param>
        public GoogleCloudLogWriter(
            Severity minSeverity,
            string serviceName = null,
            string serviceVersion = null,
            bool useGoogleCloudTimestamp = false,
            IDictionary<string, string> labels = null) : base(minSeverity)
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
            if (ex != null)
            {
                msg.AppendLine($"");
                msg.AppendLine($"    Exception Type: {ex.GetType()}");
                msg.AppendLine($"    Exception Message: {ex.Message}");
                msg.AppendLine($"    StackTrace:");
                msg.AppendLine($"        {ex.StackTrace}");
            }

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
                logEntry.Add("error", new ErrorContext(ex));

            if (_labels != null && _labels.Count > 0)
                logEntry.Add("logging.googleapis.com/labels", _labels);

            if (data != null && data.Keys.Count > 0)
                logEntry.Add("data", data as Dictionary<string, object>);

            var json = JsonSerializer.Serialize(logEntry, _jsonOptions);

            Console.WriteLine(json);
        }
    }
}