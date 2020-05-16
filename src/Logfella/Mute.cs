using System;
using System.Collections.Generic;
using Logfella.LogWriters;

namespace Logfella
{
    public static class Mute
    {
        public class MuteBuilder
        {
            internal MuteBuilder(Func<Severity, string, IDictionary<string, object>, Exception, bool> muteFilter)
            {
                MuteFilter = muteFilter;
            }

            internal readonly Func<Severity, string, IDictionary<string, object>, Exception, bool> MuteFilter;
        }

        public static MuteBuilder When(
            Func<Severity, string, IDictionary<string, object>, Exception, bool> muteFilter) =>
            new MuteBuilder(muteFilter);

        public static ILogWriter Otherwise(this MuteBuilder builder, ILogWriter logWriter) =>
            new MuteLogWriter(builder.MuteFilter, logWriter);
    }
}