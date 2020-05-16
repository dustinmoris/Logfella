using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Logfella.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestLogging(
            this IApplicationBuilder builder,
            bool isEnabled = true,
            bool logOnlyOnceAfter = false) =>
            builder.UseMiddleware<RequestLoggingMiddleware>(isEnabled, logOnlyOnceAfter);

        public static IApplicationBuilder UseRequestBasedLogWriter(
            this IApplicationBuilder builder,
            Func<HttpContext, ILogWriter> logWriterFactory) =>
            builder.UseMiddleware<RequestBasedLogWriterMiddleware>(logWriterFactory);
    }
}