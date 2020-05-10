using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Logfella.AspNetCore
{
    public class PerRequestLogWriterMiddleware
    {
        private readonly Func<HttpContext, ILogWriter> _logWriterFactory;
        private readonly RequestDelegate _next;

        public PerRequestLogWriterMiddleware(
            Func<HttpContext, ILogWriter> logWriterFactory,
            RequestDelegate next)
        {
            _logWriterFactory =
                logWriterFactory
                ?? throw new ArgumentNullException(nameof(logWriterFactory));

            _next =
                next
                ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            Log.SetAsyncLocalLogWriter(_logWriterFactory(ctx));
            await _next(ctx);
        }
    }
}