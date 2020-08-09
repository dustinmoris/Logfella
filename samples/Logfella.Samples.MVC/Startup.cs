using System;
using System.Collections.Generic;
using Logfella.AspNetCore;
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
            // This is an optional middleware which can be configured to create a new
            // `ILogWriter` instance for each incoming HTTP request. This makes it possible
            // to decorate all log entries with the HTTP context (see `AddHttpContext(ctx)`)
            // and to set a correlation ID for all log entries which belong to the same
            // HTTP request (see `AddCorrelationId(..., ...)`).
            //
            // Because this middleware requires a factory method (Func<HttpContext,ILogWriter>)
            // one has full control over how or which `ILogWriter` to use and to inspect an incoming
            // HttpRequest in order to dynamically configure the instance (e.g. check the HTTP
            // headers for an X-Correlation-Id header and use that value as `requestId`):
            app.UseRequestScopedLogWriter(
                ctx =>
                    // Note that one can use an already existing and pre-configured GoogleCloudLogWriter
                    // to extend it with additional features without having to re-create the entire
                    // object again. The GoogleCloudLogWriter is immutable and the builder methods
                    // always create a new instance, which makes it safe to use it in this way:
                    Program.DefaultLogWriter
                        .AddLabel("loggerType", "requestLogger")
                        .AddHttpContext(ctx)
                        .AddCorrelationId(
                            // Correlation ID/Request ID can either be inherited from an incoming
                            // web request in a distributed system or a new one generated:
                            ctx.Request.Headers.ContainsKey("X-Correlation-Id")
                                ? ctx.Request.Headers["X-Correlation-Id"].ToString()
                                : Guid.NewGuid().ToString("N"),
                            "requestId")); // <-- Change the key name if the default `correlationId`
                                                //     is already used for something else.

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app .UseRequestLogging(
                    options =>
                        options.ExcludeHttpHeaders = HashSet.New("Authorization", "Cookie", "X-ApiKey"))
                .UseRouting()
                .UseEndpoints(
                    endpoints => { endpoints.MapControllers(); });
        }
    }
}