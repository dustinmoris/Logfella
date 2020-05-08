using Microsoft.Extensions.Logging;

namespace Logfella.Adapters
{
    public static class LogBuilderExtensions
    {
        public static void UseLogfella(this ILoggingBuilder builder) =>
            builder
                .ClearProviders()
                .AddProvider(new LoggerAdapterProvider());
    }
}