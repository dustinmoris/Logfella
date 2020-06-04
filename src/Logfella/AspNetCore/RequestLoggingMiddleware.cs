using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Logfella.AspNetCore
{
    public class RequestLoggingMiddleware
    {
        private readonly bool _isEnabled;
        private readonly bool _logOnlyOnceAfter;
        private readonly HashSet<string> _includeHttpHeaders;
        private readonly HashSet<string> _excludeHttpHeaders;
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(
            bool isEnabled,
            bool logOnlyOnceAfter,
            HashSet<string> includeHttpHeaders,
            HashSet<string> excludeHttpHeaders,
            RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _isEnabled = isEnabled;
            _logOnlyOnceAfter = logOnlyOnceAfter;
            _includeHttpHeaders = includeHttpHeaders;
            _excludeHttpHeaders = excludeHttpHeaders;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            if (!_isEnabled)
            {
                await _next(ctx);
                return;
            }

            if (_logOnlyOnceAfter)
            {
                var before = RequestData.FromContext(ctx, _includeHttpHeaders, _excludeHttpHeaders);
                await _next(ctx);
                var after = RequestData.FromContext(ctx, _includeHttpHeaders, _excludeHttpHeaders);

                Log.Info(
                    after.ToString(),
                    new Dictionary<string, object>
                    {
                        {"categoryName", "requestLogging"},
                        {"requestLoggingType", "onceAfter"},
                        {"requestBefore", before},
                        {"requestAfter", after}
                    });
            }
            else
            {
                var before = RequestData.FromContext(ctx, _includeHttpHeaders, _excludeHttpHeaders);
                Log.Info(
                    before.ToString(),
                    new Dictionary<string, object>
                    {
                        {"categoryName", "requestLogging"},
                        {"requestLoggingType", "before"},
                        {"request", before}
                    });

                await _next(ctx);

                var after = RequestData.FromContext(ctx, _includeHttpHeaders, _excludeHttpHeaders);
                Log.Info(
                    after.ToString(),
                    new Dictionary<string, object>
                    {
                        {"categoryName", "requestLogging"},
                        {"requestLoggingType", "after"},
                        {"request", after}
                    });
            }
        }
    }
}