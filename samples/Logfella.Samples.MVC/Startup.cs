using System;
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
            // Configure a new log writer to be used across an entire request
            // and set a new GUID as correlationId:
            app.UsePerRequestLogWriter(
                ctx => new ConsoleLogWriter(
                    Severity.Debug,

                    // The key/property under which the correlation ID
                    // will be serialised in the structured logs.
                    // The default is 'correlationId' but change if it
                    // clashes with another property of the same name:
                    "correlationId",

                    // Configure a correlation ID for all logs as part
                    // of a single HTTP request. Either generate a new
                    // ID or inspect the HTTP request (e.g. X-Correlation-Id)
                    // to inherit a correlation ID from the caller:
                    ctx.Request.Query["cid"].ToString()));

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app .UseRequestLogging()
                .UseRouting()
                .UseEndpoints(
                    endpoints => { endpoints.MapControllers(); });
        }
    }
}