using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Logfella.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// <para>Logs the current HTTP request when enabled.</para>
        /// <para>Specify a list of approved HTTP headers to be included with each log entry via the <paramref name="includeHttpHeaders"/> parameter.</para>
        /// <para>Alternatively specify a list of HTTP headers which should never get logged as part of the <paramref name="excludeHttpHeaders"/> parameter.</para>
        /// <para>If both, <paramref name="includeHttpHeaders"/> and <paramref name="excludeHttpHeaders"/> are specified, then the HTTP headers will get filtered by the inclusion rule first and later the remaining headers will be filtered by the exclusion rule on top of it.</para>
        /// </summary>
        public static IApplicationBuilder UseRequestLogging(
            this IApplicationBuilder builder,
            bool isEnabled = true,
            bool logOnlyOnceAfter = false,
            HashSet<string> includeHttpHeaders = null,
            HashSet<string> excludeHttpHeaders = null) =>
            builder.UseMiddleware<RequestLoggingMiddleware>(
                isEnabled,
                logOnlyOnceAfter,
                includeHttpHeaders,
                excludeHttpHeaders);

        public static IApplicationBuilder UseRequestBasedLogWriter(
            this IApplicationBuilder builder,
            Func<HttpContext, ILogWriter> logWriterFactory) =>
            builder.UseMiddleware<RequestBasedLogWriterMiddleware>(logWriterFactory);
    }
}