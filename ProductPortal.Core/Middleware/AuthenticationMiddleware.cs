using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;
            _logger.LogInformation($"Request path: {path}");


            var publicPaths = new[] {
            "/Login",
            "/api/Auth/login",
            "/api/Auth/register",
            "/swagger",
            "/api/Auth/logout",
            "/css",
            "/js",
            "/lib"
        };

            if (publicPaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            // Token kontrolü
            var token = context.Request.Cookies["jwt"] ??
                       context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                    path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { message = "Unauthorized" });
                    return;
                }

                context.Response.Redirect("/Login/Index");
                return;
            }

            // Token varsa header'a ekle
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Request.Headers.Add("Authorization", $"Bearer {token}");
            }

            if (path.StartsWithSegments("/Login") || path.StartsWithSegments("/Auth"))
            {
                await _next(context);
                return;
            }

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(new { message = "Internal server error" });
                }
            }
        }
    }
}

