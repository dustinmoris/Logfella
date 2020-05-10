using Logfella.Adapters;
using Logfella.LogWriters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Logfella.Samples.MVC
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Configure a generic logger for logging outside the web request pipeline
            // when a per-request-logger has been configured, or a single logger
            // across the board when the per-request-logger hasn't been setup:
            Log.LogWriter = new ConsoleLogWriter(Severity.Debug);

            StartWebServer(args);
        }

        private static void StartWebServer(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // Configure the host to use Logfella when instantiating
                // new ILogger or ILogger<T> instances:
                .UseLogfella()
                .ConfigureWebHost(
                    webBuilder =>
                    {
                        webBuilder
                            .UseKestrel()
                            .UseStartup<Startup>();
                    })
                .Build()
                .Run();
    }
}