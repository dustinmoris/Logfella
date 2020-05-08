using System;
using System.Collections.Generic;
using System.Linq;

namespace Logfella.LogWriters
{
    public class MultiLogWriter : LogWriter
    {
        private readonly ILogWriter[] _logWriters;

        public MultiLogWriter(
            Severity minSeverity,
            params ILogWriter[] logWriters)
            : base(minSeverity)
        {
            _logWriters =
                logWriters
                ?? throw new ArgumentNullException(nameof(logWriters));
        }

        public MultiLogWriter(
            Severity minSeverity,
            IEnumerable<ILogWriter> logWriters)
            : base(minSeverity)
        {
            if (logWriters == null)
                throw new ArgumentNullException(nameof(logWriters));
            _logWriters = logWriters.ToArray();
        }

        protected override void WriteLog(
            Severity severity,
            string message,
            IDictionary<string, object> data,
            Exception ex)
        {
            foreach (var logWriter in _logWriters)
                logWriter.Log(severity, message, data, ex);
        }
    }
}