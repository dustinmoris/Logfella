using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Logfella.AspNetCore
{
    public class RequestLoggingMiddleware
    {
        private readonly bool _isEnabled;
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(
            bool isEnabled,
            RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _isEnabled = isEnabled;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            if (!_isEnabled)
            {
                await _next(ctx);
                return;
            }

            var before = RequestData.FromContext(ctx);
            await _next(ctx);
            var after = RequestData.FromContext(ctx);

            Log.Info(
                after.ToString(),
                new Dictionary<string, object>
                {
                    {"categoryName", "requestLogging"},
                    {"requestBefore", before},
                    {"requestAfter", after}
                });
        }
    }
}