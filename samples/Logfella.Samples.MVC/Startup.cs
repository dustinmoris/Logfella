using System;
using System.Collections.Generic;
using Logfella.AspNetCore;
using Logfella.LogWriters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Logfella.Samples.MVC
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure a factory method to generate a new ILogWriter instance
            // for each individual incoming HTTP request of the ASP.NET Core pipeline.
            // This middleware is entirely optional and not required to configure
            // Logfella in general. It mainly serves the purpose of being able to
            // set a unique "correlation ID" on an ILogWriter in order to allow
            // request based log aggregation in the Google Cloud Logging console
            // (or any other log aggregation tool in that matter):
            app.UsePerRequestLogWriter(
                ctx =>
                    new GoogleCloudLogWriter(
                        Severity.Debug,
                        "SampleAppForCSharp",
                        "v1.0.0",
                        false,
                        new Dictionary<string, string>
                            {
                                { "appName", "SampleAppForCSharp" },
                                { "appVersion", "v1.0.0" },
                                { "foo", "bar" }
                            },

                        // The key/property under which the correlation ID
                        // will be serialised in the structured logs.
                        // The default is 'correlationId' but it can be useful
                        // to set it to a different value if that name is already
                        // taken by another property which might occur in log entries:
                        "correlationId",

                        // Configure a correlation ID for all logs as part of
                        // a single HTTP request. Either generate a new unique ID
                        // (e.g. Guid.NewGuid.ToString()) or inspect the HTTP request
                        // (e.g. X-Correlation-Id HTTP header) to retrieve an existing
                        // correlation ID from another service in the chain of commands:
                        ctx.Request.Query.ContainsKey("cid")
                            ? ctx.Request.Query["cid"].ToString()
                            : Guid.NewGuid().ToString("N"))

                        // Include additional HTTP request data with each log entry (optional):
                        .IncludeHttpRequest(ctx));

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app .UseRequestLogging()
                .UseRouting()
                .UseEndpoints(
                    endpoints => { endpoints.MapControllers(); });
        }
    }
}