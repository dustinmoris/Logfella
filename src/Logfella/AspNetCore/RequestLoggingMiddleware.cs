using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Logfella.AspNetCore
{
    public class RequestLoggingOptions
    {
        public RequestLoggingOptions()
        {
            IsEnabled = true;
            LogOnlyAfter = false;
            IncludeHttpHeaders = null;
            ExcludeHttpHeaders = null;
        }

        public bool IsEnabled { get; set; }
        public bool LogOnlyAfter { get; set; }
        public HashSet<string> IncludeHttpHeaders { get; set; }
        public HashSet<string> ExcludeHttpHeaders { get; set; }
    }

    public class RequestLoggingMiddleware
    {
        private readonly RequestLoggingOptions _options;
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = new RequestLoggingOptions();
        }

        public RequestLoggingMiddleware(
            RequestLoggingOptions options,
            RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            if (!_options.IsEnabled)
            {
                await _next(ctx);
                return;
            }

            if (_options.LogOnlyAfter)
            {
                var before = RequestData.FromContext(ctx, _options.IncludeHttpHeaders, _options.ExcludeHttpHeaders);
                await _next(ctx);
                var after = RequestData.FromContext(ctx, _options.IncludeHttpHeaders, _options.ExcludeHttpHeaders);

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
                var before = RequestData.FromContext(ctx, _options.IncludeHttpHeaders, _options.ExcludeHttpHeaders);
                Log.Info(
                    before.ToString(),
                    new Dictionary<string, object>
                    {
                        {"categoryName", "requestLogging"},
                        {"requestLoggingType", "before"},
                        {"request", before}
                    });

                await _next(ctx);

                var after = RequestData.FromContext(ctx, _options.IncludeHttpHeaders, _options.ExcludeHttpHeaders);
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