using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Middleware
{
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;

        public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();
                var elapsedMilliseconds = sw.ElapsedMilliseconds;

                if (elapsedMilliseconds > 500) // 500ms üzeri logla
                {
                    var request = context.Request;
                    _logger.LogWarning(
                        "Yavaş istek: {Method} {Path} took {ElapsedMilliseconds}ms",
                        request.Method,
                        request.Path,
                        elapsedMilliseconds);
                }
            }
        }
    }
}
