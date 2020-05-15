using System;
using System.Collections.Generic;

namespace Logfella
{
    /// <inheritdoc />
    public abstract class LogWriter : ILogWriter
    {
        /// <summary>
        /// Minimum severity for a message to get logged.
        /// </summary>
        protected readonly Severity MinSeverity;

        /// <summary>
        /// Property name under which a possible correlation ID will be logged.
        /// </summary>
        protected readonly string CorrelationIdKey;

        /// <summary>
        /// Correlation ID value to aggregate a set of related logs (e.g. per HTTP request).
        /// </summary>
        protected readonly string CorrelationId;

        /// <summary>
        /// Initialises a new instance of a LogWriter with a minimum severity level.
        /// </summary>
        protected LogWriter(
            Severity minSeverity,
            string correlationIdKey = "correlationId",
            string correlationId = "")
        {
            MinSeverity = minSeverity;
            CorrelationIdKey = correlationIdKey;
            CorrelationId = correlationId;
        }

        /// <summary>
        /// <para>Convenience method to type cast into `ILogWriter` interface.</para>
        /// <para>This method is typically redundant in C# and mostly useful for F# applications only.</para>
        /// </summary>
        public ILogWriter AsLogWriter() => this;

        private static IDictionary<TKey, TValue> ToDict<TKey, TValue>(IEnumerable<Tuple<TKey, TValue>> array)
        {
            var dict = new Dictionary<TKey, TValue>();

            foreach (var (key, value) in array)
                dict.Add(key, value);

            return dict;
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

            if (!string.IsNullOrEmpty(CorrelationId))
            {
                data ??= new Dictionary<string, object>();
                data.Add(CorrelationIdKey, CorrelationId);
            }

            WriteLog(severity, message, data, ex);
        }

        /// <inheritdoc />
        public void Log(Severity severity, string message, IDictionary<string, object> data, Exception ex) =>
            WriteLogIfSeverity(severity, message, data, ex);

        /// <inheritdoc />
        public void Default(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Default, message, ToDict(data));

        /// <inheritdoc />
        public void Default(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Default, message, ToDict(data), ex);

        /// <inheritdoc />
        public void Default(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Default, message, data, ex);

        /// <inheritdoc />
        public void Debug(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Debug, message, ToDict(data));

        /// <inheritdoc />
        public void Debug(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Debug, message, ToDict(data), ex);

        /// <inheritdoc />
        public void Debug(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Debug, message, data, ex);

        /// <inheritdoc />
        public void Info(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Info, message, ToDict(data));

        /// <inheritdoc />
        public void Info(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Info, message, ToDict(data), ex);

        /// <inheritdoc />
        public void Info(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Info, message, data, ex);

        /// <inheritdoc />
        public void Notice(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Notice, message, ToDict(data));

        /// <inheritdoc />
        public void Notice(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Notice, message, ToDict(data), ex);

        /// <inheritdoc />
        public void Notice(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Notice, message, data, ex);

        /// <inheritdoc />
        public void Warning(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Warning, message, ToDict(data));

        /// <inheritdoc />
        public void Warning(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Warning, message, ToDict(data), ex);

        /// <inheritdoc />
        public void Warning(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Warning, message, data, ex);

        /// <inheritdoc />
        public void Error(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Error, message, ToDict(data));

        /// <inheritdoc />
        public void Error(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Error, message, ToDict(data), ex);

        /// <inheritdoc />
        public void Error(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Error, message, data, ex);

        /// <inheritdoc />
        public void Critical(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Critical, message, ToDict(data));

        /// <inheritdoc />
        public void Critical(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Critical, message, ToDict(data), ex);

        /// <inheritdoc />
        public void Critical(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Critical, message, data, ex);

        /// <inheritdoc />
        public void Alert(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Alert, message, ToDict(data));

        /// <inheritdoc />
        public void Alert(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Alert, message, ToDict(data), ex);

        /// <inheritdoc />
        public void Alert(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Alert, message, data, ex);

        /// <inheritdoc />
        public void Emergency(string message, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Emergency, message, ToDict(data));

        /// <inheritdoc />
        public void Emergency(string message, Exception ex, params Tuple<string, object>[] data) =>
            WriteLogIfSeverity(Severity.Emergency, message, ToDict(data), ex);

        /// <inheritdoc />
        public void Emergency(string message, IDictionary<string, object> data, Exception ex = null) =>
            WriteLogIfSeverity(Severity.Emergency, message, data, ex);
    }
}