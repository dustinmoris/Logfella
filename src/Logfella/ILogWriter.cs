using System;
using System.Collections.Generic;

namespace Logfella
{
    /// <summary>
    /// Structured Log Writer.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Logs a message and a set of given data and/or an exception with the given severity.
        /// </summary>
        void Log(Severity severity, string message, IDictionary<string, object> data, Exception ex = null);

        /// <summary>
        /// (0) The log entry has no assigned severity level.
        /// </summary>
        void Default(string message, params Tuple<string, object>[] data);

        /// <summary>
        /// (0) The log entry has no assigned severity level.
        /// </summary>
        void Default(string message, Exception ex, params Tuple<string, object>[] data);

        /// <summary>
        /// (0) The log entry has no assigned severity level.
        /// </summary>
        void Default(string message, IDictionary<string, object> data, Exception ex = null);

        /// <summary>
        /// (100) Debug or trace information.
        /// </summary>
        void Debug(string message, params Tuple<string, object>[] data);

        /// <summary>
        /// (100) Debug or trace information.
        /// </summary>
        void Debug(string message, Exception ex, params Tuple<string, object>[] data);

        /// <summary>
        /// (100) Debug or trace information.
        /// </summary>
        void Debug(string message, IDictionary<string, object> data, Exception ex = null);

        /// <summary>
        /// (200) Routine information, such as ongoing status or performance.
        /// </summary>
        void Info(string message, params Tuple<string, object>[] data);

        /// <summary>
        /// (200) Routine information, such as ongoing status or performance.
        /// </summary>
        void Info(string message, Exception ex, params Tuple<string, object>[] data);

        /// <summary>
        /// (200) Routine information, such as ongoing status or performance.
        /// </summary>
        void Info(string message, IDictionary<string, object> data, Exception ex = null);

        /// <summary>
        /// (300) Normal but significant events, such as start up, shut down, or a configuration change.
        /// </summary>
        void Notice(string message, params Tuple<string, object>[] data);

        /// <summary>
        /// (300) Normal but significant events, such as start up, shut down, or a configuration change.
        /// </summary>
        void Notice(string message, Exception ex, params Tuple<string, object>[] data);

        /// <summary>
        /// (300) Normal but significant events, such as start up, shut down, or a configuration change.
        /// </summary>
        void Notice(string message, IDictionary<string, object> data, Exception ex = null);

        /// <summary>
        /// (400) Warning events might cause problems.
        /// </summary>
        void Warning(string message, params Tuple<string, object>[] data);

        /// <summary>
        /// (400) Warning events might cause problems.
        /// </summary>
        void Warning(string message, Exception ex, params Tuple<string, object>[] data);

        /// <summary>
        /// (400) Warning events might cause problems.
        /// </summary>
        void Warning(string message, IDictionary<string, object> data, Exception ex = null);

        /// <summary>
        /// (500) Error events are likely to cause problems.
        /// </summary>
        void Error(string message, params Tuple<string, object>[] data);

        /// <summary>
        /// (500) Error events are likely to cause problems.
        /// </summary>
        void Error(string message, Exception ex, params Tuple<string, object>[] data);

        /// <summary>
        /// (500) Error events are likely to cause problems.
        /// </summary>
        void Error(string message, IDictionary<string, object> data, Exception ex = null);

        /// <summary>
        /// (600) Critical events cause more severe problems or outages.
        /// </summary>
        void Critical(string message, params Tuple<string, object>[] data);

        /// <summary>
        /// (600) Critical events cause more severe problems or outages.
        /// </summary>
        void Critical(string message, Exception ex, params Tuple<string, object>[] data);

        /// <summary>
        /// (600) Critical events cause more severe problems or outages.
        /// </summary>
        void Critical(string message, IDictionary<string, object> data, Exception ex = null);

        /// <summary>
        /// (700) A person must take an action immediately.
        /// </summary>
        void Alert(string message, params Tuple<string, object>[] data);

        /// <summary>
        /// (700) A person must take an action immediately.
        /// </summary>
        void Alert(string message, Exception ex, params Tuple<string, object>[] data);

        /// <summary>
        /// (700) A person must take an action immediately.
        /// </summary>
        void Alert(string message, IDictionary<string, object> data, Exception ex = null);

        /// <summary>
        /// (800) One or more systems are unusable.
        /// </summary>
        void Emergency(string message, params Tuple<string, object>[] data);

        /// <summary>
        /// (800) One or more systems are unusable.
        /// </summary>
        void Emergency(string message, Exception ex, params Tuple<string, object>[] data);

        /// <summary>
        /// (800) One or more systems are unusable.
        /// </summary>
        void Emergency(string message, IDictionary<string, object> data, Exception ex = null);
    }
}