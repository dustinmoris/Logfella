using System.Collections.Generic;
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
            // Configure an ILogWriter instance for this application.
            // This method should only be called once during application startup.
            // Normally a program would decide which kind of logger it wants to
            // create based on environment settings during the start-up procedures.
            //
            // In an ASP.NET Core application you can configure additional "per-request"
            // ILogWriter instances by configuring additional middleware inside Startup.cs,
            // however those request-based log writers only exist for the scope of a HTTP
            // request and any logs which need to be written outside the ASP.NET Core request
            // pipeline will continue using the global log writer configured here:
            Log.SetGlobalLogWriter(
                new GoogleCloudLogWriter(
                    Severity.Debug,                            // Set the minimum log level
                    "SampleAppForCSharp",          // The service name and service version are being
                    "v1.0.0",                     // used for Google Error Reporting.
                    false,              // Set to true if Google Cloud Logging should determine the log timestamp itself.
                    new Dictionary<string, string>
                    {
                        { "appName", "SampleAppForCSharp" },   // Labels which all logs will be decorated with.
                        { "appVersion", "v1.0.0" },            // Use this as a mechanism to easily filter logs in GCP.
                        { "loggerType", "global" },            // Example, set the version of the service to easily determine
                        { "foo", "bar" }                       // which deployment might have caused an influx of errors, etc.
                    }));

            StartWebServer(args);
        }

        private static void StartWebServer(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // Configure Microsoft.Extensions.Logging to use Logfella
                // when creating new ILogger or ILogger<T> instances.
                //
                // This is not necessarily required, but then logs from other middleware
                // will not write to the same log writer as the one set above.
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