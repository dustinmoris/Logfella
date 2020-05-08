using Microsoft.Extensions.Hosting;

namespace Logfella.Adapters
{
    public static class HostBuilderExtensions
    {
        public static void UseLogfella(this IHostBuilder builder) =>
            builder.ConfigureLogging(b => b.UseLogfella());
    }
}