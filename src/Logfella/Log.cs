using System;
using System.Collections.Generic;
using Logfella.LogWriters;

namespace Logfella
{
    public static class Log
    {
        private static ILogWriter _logWriter = new NullLogWriter(Severity.Info);

        public static ILogWriter GetLogWriter() => _logWriter;

        public static void SetLogWriter(ILogWriter logWriter)
        {
            _logWriter = logWriter ?? throw new ArgumentNullException(nameof(logWriter));
        }

        public static void Default(string message, params Tuple<string, object>[] data) =>
            _logWriter.Default(message, data);

        public static void Default(string message, Exception ex, params Tuple<string, object>[] data) =>
            _logWriter.Default(message, ex, data);

        public static void Default(string message, IDictionary<string, object> data, Exception ex = null) =>
            _logWriter.Default(message, data, ex);

        public static void Debug(string message, params Tuple<string, object>[] data) =>
            _logWriter.Debug(message, data);

        public static void Debug(string message, Exception ex, params Tuple<string, object>[] data) =>
            _logWriter.Debug(message, ex, data);

        public static void Debug(string message, IDictionary<string, object> data, Exception ex = null) =>
            _logWriter.Debug(message, data, ex);

        public static void Info(string message, params Tuple<string, object>[] data) =>
            _logWriter.Info(message, data);

        public static void Info(string message, Exception ex, params Tuple<string, object>[] data) =>
            _logWriter.Info(message, ex, data);

        public static void Info(string message, IDictionary<string, object> data, Exception ex = null) =>
            _logWriter.Info(message, data, ex);

        public static void Notice(string message, params Tuple<string, object>[] data) =>
            _logWriter.Notice(message, data);

        public static void Notice(string message, Exception ex, params Tuple<string, object>[] data) =>
            _logWriter.Notice(message, ex, data);

        public static void Notice(string message, IDictionary<string, object> data, Exception ex = null) =>
            _logWriter.Notice(message, data, ex);

        public static void Warning(string message, params Tuple<string, object>[] data) =>
            _logWriter.Warning(message, data);

        public static void Warning(string message, Exception ex, params Tuple<string, object>[] data) =>
            _logWriter.Warning(message, ex, data);

        public static void Warning(string message, IDictionary<string, object> data, Exception ex = null) =>
            _logWriter.Warning(message, data, ex);

        public static void Error(string message, params Tuple<string, object>[] data) =>
            _logWriter.Error(message, data);

        public static void Error(string message, Exception ex, params Tuple<string, object>[] data) =>
            _logWriter.Error(message, ex, data);

        public static void Error(string message, IDictionary<string, object> data, Exception ex = null) =>
            _logWriter.Error(message, data, ex);

        public static void Critical(string message, params Tuple<string, object>[] data) =>
            _logWriter.Critical(message, data);

        public static void Critical(string message, Exception ex, params Tuple<string, object>[] data) =>
            _logWriter.Critical(message, ex, data);

        public static void Critical(string message, IDictionary<string, object> data, Exception ex = null) =>
            _logWriter.Critical(message, data, ex);

        public static void Alert(string message, params Tuple<string, object>[] data) =>
            _logWriter.Alert(message, data);

        public static void Alert(string message, Exception ex, params Tuple<string, object>[] data) =>
            _logWriter.Alert(message, ex, data);

        public static void Alert(string message, IDictionary<string, object> data, Exception ex = null) =>
            _logWriter.Alert(message, data, ex);

        public static void Emergency(string message, params Tuple<string, object>[] data) =>
            _logWriter.Emergency(message, data);

        public static void Emergency(string message, Exception ex, params Tuple<string, object>[] data) =>
            _logWriter.Emergency(message, ex, data);

        public static void Emergency(string message, IDictionary<string, object> data, Exception ex = null) =>
            _logWriter.Emergency(message, data, ex);
    }
}