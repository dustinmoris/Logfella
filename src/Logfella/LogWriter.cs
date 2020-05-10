using System;
using System.Collections.Generic;
using Logfella.Extensions;

namespace Logfella
{
    /// <inheritdoc />
    public abstract class LogWriter : ILogWriter
    {
        /// <summary>
        /// Minimum severity for a message to get logged.
        /// </summary>
        protected readonly Severity MinSeverity;

        private readonly string _correlationIdKey;
        private readonly string _correlationId;

        /// <summary>
        /// Initialises a new instance of a LogWriter with a minimum severity level.
        /// </summary>
        protected LogWriter(
            Severity minSeverity,
            string correlationIdKey = "correlationId",
            string correlationId = "")
        {
            MinSeverity = minSeverity;
            _correlationIdKey = correlationIdKey;
            _correlationId = correlationId;
        }

        /// <summary>
        /// Logs a message and a set of given data and/or an exception with the given severity.
        /// </summary>
        protected abstract void WriteLog(
            Severity severity,
            string message,
            IDictionary<string, object> data,
            Exception ex);

        /// <summary>
        /// Logs a message only if the severity meets the minimum severity set for the log writer.
        /// </summary>
        protected virtual void WriteLogIfSeverity(
            Severity severity,
            string message,
            IDictionary<string, object> data,
            Exception ex = null)
        {
            if (severity < MinSeverity)
                return;

            if (!string.IsNullOrEmpty(_correlationId))
            {
                data ??= new Dictionary<string, object>();
                data.Add(_correlationIdKey, _correlationId);
            }

            WriteLog(severity, message, data, ex);
        }

        /// <inheritdoc />
        public void Log(Severity severity, string message, IDictionary<string, object> data, Exception ex) =>
            WriteLogIfSeverity(severity, message, data, ex);

        /// <inheritdoc />
        public void Default(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Default, message, data.ToDict());

        /// <inheritdoc />
        public void Default(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Default, message, data.ToDict(), ex);

        /// <inheritdoc />
        public void Default(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Default, message, data, ex);

        /// <inheritdoc />
        public void Debug(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Debug, message, data.ToDict());

        /// <inheritdoc />
        public void Debug(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Debug, message, data.ToDict(), ex);

        /// <inheritdoc />
        public void Debug(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Debug, message, data, ex);

        /// <inheritdoc />
        public void Info(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Info, message, data.ToDict());

        /// <inheritdoc />
        public void Info(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Info, message, data.ToDict(), ex);

        /// <inheritdoc />
        public void Info(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Info, message, data, ex);

        /// <inheritdoc />
        public void Notice(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Notice, message, data.ToDict());

        /// <inheritdoc />
        public void Notice(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Notice, message, data.ToDict(), ex);

        /// <inheritdoc />
        public void Notice(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Notice, message, data, ex);

        /// <inheritdoc />
        public void Warning(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Warning, message, data.ToDict());

        /// <inheritdoc />
        public void Warning(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Warning, message, data.ToDict(), ex);

        /// <inheritdoc />
        public void Warning(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Warning, message, data, ex);

        /// <inheritdoc />
        public void Error(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Error, message, data.ToDict());

        /// <inheritdoc />
        public void Error(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Error, message, data.ToDict(), ex);

        /// <inheritdoc />
        public void Error(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Error, message, data, ex);

        /// <inheritdoc />
        public void Critical(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Critical, message, data.ToDict());

        /// <inheritdoc />
        public void Critical(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Critical, message, data.ToDict(), ex);

        /// <inheritdoc />
        public void Critical(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Critical, message, data, ex);

        /// <inheritdoc />
        public void Alert(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Alert, message, data.ToDict());

        /// <inheritdoc />
        public void Alert(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Alert, message, data.ToDict(), ex);

        /// <inheritdoc />
        public void Alert(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Alert, message, data, ex);

        /// <inheritdoc />
        public void Emergency(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Emergency, message, data.ToDict());

        /// <inheritdoc />
        public void Emergency(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Emergency, message, data.ToDict(), ex);

        /// <inheritdoc />
        public void Emergency(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Emergency, message, data, ex);
    }
}