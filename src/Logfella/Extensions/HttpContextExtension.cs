using Microsoft.AspNetCore.Http;

namespace Logfella.Extensions
{
    public static class HttpContextExtension
    {
        public static string RequestUrl(this HttpContext ctx) =>
            $"{ctx.Request.Scheme}://{ctx.Request.Host.Value}/{ctx.Request.PathBase.Value}{ctx.Request.Path.Value}{ctx.Request.QueryString.Value}";
    }
}