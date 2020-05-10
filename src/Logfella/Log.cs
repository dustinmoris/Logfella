using System;
using System.Collections.Generic;
using System.Threading;
using Logfella.LogWriters;

namespace Logfella
{
    public static class Log
    {
        private static ILogWriter _genericLogWriter =
            new NullLogWriter(Severity.Default);

        private static readonly AsyncLocal<ILogWriter> AsyncLocalLogWriter =
            new AsyncLocal<ILogWriter>();

        public static ILogWriter LogWriter
        {
            get => AsyncLocalLogWriter.Value ?? _genericLogWriter;
            set => _genericLogWriter = value ?? throw new ArgumentNullException(nameof(value));
        }

        internal static void SetAsyncLocalLogWriter(ILogWriter logWriter)
        {
            AsyncLocalLogWriter.Value =
                logWriter
                ?? throw new ArgumentNullException(nameof(logWriter));
        }

        public static void Default(string message, params Tuple<string, object>[] data) =>
            LogWriter.Default(message, data);

        public static void Default(string message, Exception ex, params Tuple<string, object>[] data) =>
            LogWriter.Default(message, ex, data);

        public static void Default(string message, IDictionary<string, object> data, Exception ex = null) =>
            LogWriter.Default(message, data, ex);

        public static void Debug(string message, params Tuple<string, object>[] data) =>
            LogWriter.Debug(message, data);

        public static void Debug(string message, Exception ex, params Tuple<string, object>[] data) =>
            LogWriter.Debug(message, ex, data);

        public static void Debug(string message, IDictionary<string, object> data, Exception ex = null) =>
            LogWriter.Debug(message, data, ex);

        public static void Info(string message, params Tuple<string, object>[] data) =>
            LogWriter.Info(message, data);

        public static void Info(string message, Exception ex, params Tuple<string, object>[] data) =>
            LogWriter.Info(message, ex, data);

        public static void Info(string message, IDictionary<string, object> data, Exception ex = null) =>
            LogWriter.Info(message, data, ex);

        public static void Notice(string message, params Tuple<string, object>[] data) =>
            LogWriter.Notice(message, data);

        public static void Notice(string message, Exception ex, params Tuple<string, object>[] data) =>
            LogWriter.Notice(message, ex, data);

        public static void Notice(string message, IDictionary<string, object> data, Exception ex = null) =>
            LogWriter.Notice(message, data, ex);

        public static void Warning(string message, params Tuple<string, object>[] data) =>
            LogWriter.Warning(message, data);

        public static void Warning(string message, Exception ex, params Tuple<string, object>[] data) =>
            LogWriter.Warning(message, ex, data);

        public static void Warning(string message, IDictionary<string, object> data, Exception ex = null) =>
            LogWriter.Warning(message, data, ex);

        public static void Error(string message, params Tuple<string, object>[] data) =>
            LogWriter.Error(message, data);

        public static void Error(string message, Exception ex, params Tuple<string, object>[] data) =>
            LogWriter.Error(message, ex, data);

        public static void Error(string message, IDictionary<string, object> data, Exception ex = null) =>
            LogWriter.Error(message, data, ex);

        public static void Critical(string message, params Tuple<string, object>[] data) =>
            LogWriter.Critical(message, data);

        public static void Critical(string message, Exception ex, params Tuple<string, object>[] data) =>
            LogWriter.Critical(message, ex, data);

        public static void Critical(string message, IDictionary<string, object> data, Exception ex = null) =>
            LogWriter.Critical(message, data, ex);

        public static void Alert(string message, params Tuple<string, object>[] data) =>
            LogWriter.Alert(message, data);

        public static void Alert(string message, Exception ex, params Tuple<string, object>[] data) =>
            LogWriter.Alert(message, ex, data);

        public static void Alert(string message, IDictionary<string, object> data, Exception ex = null) =>
            LogWriter.Alert(message, data, ex);

        public static void Emergency(string message, params Tuple<string, object>[] data) =>
            LogWriter.Emergency(message, data);

        public static void Emergency(string message, Exception ex, params Tuple<string, object>[] data) =>
            LogWriter.Emergency(message, ex, data);

        public static void Emergency(string message, IDictionary<string, object> data, Exception ex = null) =>
            LogWriter.Emergency(message, data, ex);
    }
}