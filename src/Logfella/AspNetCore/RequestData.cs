using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Logfella.AspNetCore
{
    public class RequestData
    {
        public string ClientIPAddress { get; set; }
        public string Scheme { get; set; }
        public string Protocol { get; set; }
        public string Verb { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public override string ToString() =>
            $"Incoming: {Protocol} {Verb} {Scheme}://{Host}{Path}{Query} from {ClientIPAddress}";

        public static RequestData FromContext(HttpContext ctx)
        {
            var req = ctx.Request;
            var headers = new Dictionary<string, string>();

            foreach (var key in req.Headers.Keys.OrderBy(k => k))
            {
                var value = req.Headers[key];
                headers.Add(key, value.ToString());
            }

            return new RequestData
            {
                ClientIPAddress = ctx.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
                Scheme = req.Scheme.ToLower(),
                Protocol = ctx.Request.Protocol,
                Verb = req.Method.ToUpper(),
                Host = req.Host.Value.TrimEnd('/'),
                Path = req.Path.Value,
                Query = req.QueryString.HasValue ? req.QueryString.Value : "",
                Headers = headers
            };
        }
    }
}