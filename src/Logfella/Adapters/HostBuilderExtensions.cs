using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Logfella.Adapters
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseLogfella(this IHostBuilder builder) =>
            builder
                .ConfigureServices(
                    (_, svc) => svc.TryAddTransient(
                        __ => Log.GetLogWriter()))
                .ConfigureLogging(b => b.UseLogfella());
    }
}