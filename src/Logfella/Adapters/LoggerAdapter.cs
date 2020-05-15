using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Logfella.Adapters
{
    public class LoggerAdapter : ILogger
    {
        private sealed class NoOpDisposable : IDisposable
        {
            internal static readonly NoOpDisposable Instance = new NoOpDisposable();
            private NoOpDisposable() { }
            public void Dispose() { }
        }

        private const string OriginalFormatKey = "{OriginalFormat}";
        private readonly string _category;

        public LoggerAdapter(string category)
        {
            _category = category;
        }

        private static Severity MapToSeverity(LogLevel logLevel) =>
            logLevel switch
            {
                LogLevel.Trace => Severity.Debug,
                LogLevel.Debug => Severity.Debug,
                LogLevel.Information => Severity.Info,
                LogLevel.Warning => Severity.Warning,
                LogLevel.Error => Severity.Error,
                LogLevel.Critical => Severity.Critical,
                _ => Severity.Default
            };

        private static bool HasSomethingToLog(IEnumerable<KeyValuePair<string, object>> items)
        {
            using var iterator = items.GetEnumerator();

            // Return false if there's nothing there:
            if (!iterator.MoveNext())
                return false;

            // If the first entry is not the original format then we want to log the items,
            // OR if the first entry *is* the original format but there is more items after
            // then we also want to log the items:
            return iterator.Current.Key != OriginalFormatKey || iterator.MoveNext();
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var logWriter = Logfella.Log.GetLogWriter();
            var severity = MapToSeverity(logLevel);
            var message = formatter(state, exception);
            var data = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(_category))
                data.Add("CategoryName", _category);

            if (state is IEnumerable<KeyValuePair<string, object>> items
                && HasSomethingToLog(items))
            {
                foreach (var kvp in items)
                {
                    var (key, value) = kvp;

                    if (string.IsNullOrEmpty(key))
                        continue;

                    if (key == OriginalFormatKey)
                        key = "messageTemplate";

                    if (char.IsDigit(key[0]))
                        key = "_" + key;

                    data.Add(key, value?.ToString() ?? "");
                }
            }

            logWriter.Log(
                severity,
                message,
                data,
                exception);
        }

        // Always enable the adapter.
        // The minimum log level is controlled via the ILogWriter instance.
        public bool IsEnabled(LogLevel logLevel) =>
            logLevel != LogLevel.None;

        // No support for scopes needed.
        public IDisposable BeginScope<TState>(TState state) =>
            NoOpDisposable.Instance;
    }
}