using Microsoft.Extensions.Logging;

namespace Logfella.Adapters
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder UseLogfella(this ILoggingBuilder builder) =>
            builder
                .ClearProviders()
                .AddProvider(new LoggerAdapterProvider());
    }
}