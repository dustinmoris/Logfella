using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Logfella.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// <para>Logs the current HTTP request when enabled.</para>
        /// </summary>
        public static IApplicationBuilder UseRequestLogging(
            this IApplicationBuilder builder) =>
            builder.UseMiddleware<RequestLoggingMiddleware>();

        /// <summary>
        /// <para>Logs the current HTTP request when enabled.</para>
        /// </summary>
        public static IApplicationBuilder UseRequestLogging(
            this IApplicationBuilder builder,
            Action<RequestLoggingOptions> configureOptions)
        {
            var options = new RequestLoggingOptions();
            configureOptions(options);
            return builder.UseMiddleware<RequestLoggingMiddleware>(options);
        }

        /// <summary>
        /// <para>Uses a request scoped log writer when enabled.</para>
        /// </summary>
        public static IApplicationBuilder UseRequestScopedLogWriter(
            this IApplicationBuilder builder,
            Func<HttpContext, ILogWriter> logWriterFactory) =>
            builder.UseMiddleware<RequestScopedLogWriterMiddleware>(logWriterFactory);
    }
}