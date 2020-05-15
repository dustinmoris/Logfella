using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Logfella.Adapters
{
    public static class AdapterExtensions
    {
        public static ILoggingBuilder UseLogfella(this ILoggingBuilder builder) =>
            builder
                .ClearProviders()
                .AddProvider(new LoggerAdapterProvider());

        public static IHostBuilder UseLogfella(this IHostBuilder builder) =>
            builder
                .ConfigureServices(
                    (_, svc) => svc.TryAddTransient(
                        __ => Log.GetLogWriter()))
                .ConfigureLogging(b => b.UseLogfella());
    }
}