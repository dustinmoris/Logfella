using System;
using System.Collections.Generic;
using Logfella.Adapters;
using Logfella.LogWriters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Logfella.Samples.MVC
{
    public static class Program
    {
        // Dummy data, in a real world app you'd load these values
        // from a config source or dynamically read from the .csproj
        // settings:
        private const string AppName = "SampleAppCSharp";
        private const string AppVersion = "v1.0.0";

        // Generate a new GoogleCloudLogWriter with the help of
        // the builder methods by dynamically adding more
        // features to the instance. These builder methods allow
        // one to add features or configurations based on conditions
        // in a nice fluent way:
        public static readonly GoogleCloudLogWriter DefaultLogWriter =
            GoogleCloudLogWriter
                .Create(Severity.Debug) // Min severity to write logs
                .AddServiceContext(     // Configure service context for Google Error Reporting
                    AppName,
                    AppVersion)
                .AddLabels(
                    new Dictionary<string, string>
                    {                                // Configure additional labels which will be
                        {"appName", AppName},        // automatically added to all log entries.
                        {"appVersion", AppVersion}   // Use labels to make log filtering & analysis easier
                    });                              // (e.g. which with app version did new errors start?)

        public static void Main(string[] args)
        {
            // Set the default ILogWriter instance.
            // This method should ideally only be invoked once during application startup:
            Log.SetDefaultLogWriter(DefaultLogWriter);

            // Write a log entry:
            Log.Info("Starting web server...");

            StartWebServer(args);
        }

        private static void StartWebServer(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // This is only required to configure an adapter so that `Microsoft.Extensions.Logging.ILogger`
                // objects will use the above configured ILogWriter for all logs:
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