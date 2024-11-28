using Microsoft.AspNetCore.Http;
using ProductPortal.Core.Utilities.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Middleware
{
    public class RateLimitingMiddleware
    {
        private static readonly Dictionary<string, TokenBucket> _buckets = new();
        private readonly RequestDelegate _next;

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var bucket = GetBucket(ipAddress);

            if (!bucket.TryTake())
            {
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsJsonAsync(new { message = "Rate limit exceeded" });
                return;
            }

            await _next(context);
        }

        private TokenBucket GetBucket(string key)
        {
            if (!_buckets.ContainsKey(key))
                _buckets[key] = new TokenBucket(100, TimeSpan.FromMinutes(1));

            return _buckets[key];
        }
    }
}