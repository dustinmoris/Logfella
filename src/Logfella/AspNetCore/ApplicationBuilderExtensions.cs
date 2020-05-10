using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Logfella.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestLogging(
            this IApplicationBuilder builder,
            bool isEnabled = true) =>
            builder.UseMiddleware<RequestLoggingMiddleware>(isEnabled);

        public static IApplicationBuilder UsePerRequestLogWriter(
            this IApplicationBuilder builder,
            Func<HttpContext, ILogWriter> logWriterFactory) =>
            builder.UseMiddleware<PerRequestLogWriterMiddleware>(logWriterFactory);
    }
}