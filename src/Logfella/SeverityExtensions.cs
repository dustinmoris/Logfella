using System;

namespace Logfella
{
    /// <summary>
    /// Helper methods to parse a severity from and to strings.
    /// </summary>
    public static class SeverityExtensions
    {
        /// <summary>
        /// Returns the string value of the severity level.
        /// </summary>
        public static string String(this Severity severity) =>
            severity.ToString().ToUpperInvariant();

        /// <summary>
        /// Parses a string value into a Severity enum value.
        /// <exception cref="ArgumentOutOfRangeException">Throws an exception when the string value cannot be matched to a valid severity level.</exception>
        /// </summary>
        public static Severity ParseSeverity(this string value)
        {
            if (Enum.TryParse(value, true, out Severity severity))
                return severity;

            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"The provided string '{value}' is not a valid Severity value.");
        }
    }
}