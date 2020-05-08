using System;
using System.Collections.Generic;
using System.Text;

namespace Logfella.LogWriters
{
    public class ConsoleLogWriter : LogWriter
    {
        public ConsoleLogWriter(Severity minSeverity)
            : base(minSeverity)
        {
        }

        protected override void WriteLog(
            Severity severity,
            string message,
            IDictionary<string, object> data,
            Exception ex)
        {
            var sb = new StringBuilder();
            sb.Append($"{DateTimeOffset.UtcNow:yyy-MM-dd hh:mm:ss.fff}");
            sb.Append($" [{severity.ToString()}] ");
            sb.Append(message);

            while (ex != null)
            {
                sb.AppendLine($"");
                sb.AppendLine($"    Exception Type: {ex.GetType()}");
                sb.AppendLine($"    Exception Message: {ex.Message}");
                sb.AppendLine($"    StackTrace:");
                sb.AppendLine($"        {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    sb.AppendLine($"");
                    sb.AppendLine($"---- Inner Exception ----");
                }

                ex = ex.InnerException;
            }

            var originalForegroundColour = Console.ForegroundColor;
            var originalBackgroundColour = Console.BackgroundColor;
            var (bg, colour) = PickColour(severity);
            if (bg.HasValue)
                Console.BackgroundColor = bg.Value;
            Console.ForegroundColor = colour;
            Console.WriteLine(sb.ToString());
            Console.ForegroundColor = originalForegroundColour;
            Console.BackgroundColor = originalBackgroundColour;
        }

        private static (ConsoleColor?, ConsoleColor) PickColour(Severity severity) =>
            severity switch
            {
                Severity.Debug => (null, ConsoleColor.Gray),
                Severity.Info => (null, ConsoleColor.White),
                Severity.Notice => (null, ConsoleColor.Green),
                Severity.Warning => (null, ConsoleColor.Yellow),
                Severity.Error => (null, ConsoleColor.DarkMagenta),
                Severity.Critical => (null, ConsoleColor.Magenta),
                Severity.Alert =>  (null, ConsoleColor.Red),
                Severity.Emergency => (ConsoleColor.Red, ConsoleColor.White),
                _ => (null, ConsoleColor.White)
            };
    }
}