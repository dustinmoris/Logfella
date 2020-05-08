using System;
using System.Collections.Generic;

namespace Logfella.LogWriters
{
    public class NullLogWriter : LogWriter
    {
        public NullLogWriter(Severity minSeverity)
            : base(minSeverity)
        {
        }

        protected override void WriteLog(
            Severity severity,
            string message,
            IDictionary<string, object> data,
            Exception ex)
        {
        }
    }
}