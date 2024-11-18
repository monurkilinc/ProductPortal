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

            // Auth ile ilgili path'leri bypass et
            if (path.StartsWithSegments("/Login") ||
                path.StartsWithSegments("/Auth") ||
                path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // Token kontrolü
            var token = context.Request.Cookies["jwt"] ??
                       context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No token found. Redirecting to login page.");
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

            //if (!context.User.Identity.IsAuthenticated)
            //{
            //    _logger.LogWarning("User not authenticated. Redirecting to login page.");
            //    context.Response.Redirect("/Login/Index");
            //    return;
            //}

            // Admin kontrolü
            if (path.StartsWithSegments("/Admin"))
            {
                var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole != "Admin")
                {
                    _logger.LogWarning($"Unauthorized access attempt to admin area by user with role: {userRole}");
                    context.Response.Redirect("/Login/Index");
                    return;
                }
            }

            await _next(context);
        }
    }
}

