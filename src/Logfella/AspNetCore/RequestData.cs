using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Logfella.AspNetCore
{
    public class RequestData
    {
        private RequestData()
        {
        }

        public string ClientIPAddress { get; private set; }
        public string Scheme { get; private set; }
        public string Protocol { get; private set; }
        public string Verb { get; private set; }
        public string Host { get; private set; }
        public string Path { get; private set; }
        public string Query { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }

        public override string ToString() =>
            $"Incoming: {Protocol} {Verb} {Scheme}://{Host}{Path}{Query} from {ClientIPAddress}";

        public static RequestData FromContext(
            HttpContext ctx,
            HashSet<string> includeHttpHeaders,
            HashSet<string> excludeHttpHeaders)
        {
            var req = ctx.Request;
            var headers = new Dictionary<string, string>();

            bool FilterByInclusion(string k) => includeHttpHeaders == null || includeHttpHeaders.Contains(k);
            bool FilterByExclusion(string k) => excludeHttpHeaders == null || !excludeHttpHeaders.Contains(k);

            foreach (var key in
                req.Headers.Keys
                    .Where(FilterByInclusion)
                    .Where(FilterByExclusion)
                    .OrderBy(k => k))
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