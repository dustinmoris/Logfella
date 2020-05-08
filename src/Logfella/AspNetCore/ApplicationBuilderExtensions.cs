using Microsoft.AspNetCore.Builder;

namespace Logfella.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestLogging(
            this IApplicationBuilder builder,
            bool isEnabled = true)
        {
            builder.UseMiddleware<RequestLoggingMiddleware>(isEnabled);
            return builder;
        }
    }
}