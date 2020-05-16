using System;
using System.Collections.Generic;

namespace Logfella.LogWriters
{
    public sealed class MuteLogWriter : LogWriter
    {
        private readonly Func<Severity, string, IDictionary<string, object>, Exception, bool> _muteFilter;
        private readonly ILogWriter _logWriter;

        public MuteLogWriter(
            Func<Severity, string, IDictionary<string, object>, Exception, bool> muteFilter,
            ILogWriter logWriter)
            : base(Severity.Default)
        {
            _muteFilter =
                muteFilter
                ?? throw new ArgumentNullException(nameof(muteFilter));

            _logWriter =
                logWriter
                ?? throw new ArgumentNullException(nameof(logWriter));
        }

        protected override void WriteLog(
            Severity severity,
            string message,
            IDictionary<string, object> data,
            Exception ex)
        {
            var mute = _muteFilter(severity, message, data, ex);

            if (!mute)
                _logWriter.Log(severity, message, data, ex);
        }
    }
}