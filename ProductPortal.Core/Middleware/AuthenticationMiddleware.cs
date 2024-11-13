using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(!context.User.Identity.IsAuthenticated && 
                !context.Request.Path.StartsWithSegments("/Auth") &&
                !context.Request.Path.StartsWithSegments("/api/auth"))
            {
                context.Response.Redirect("/Auth/Login");
                return;
            }
            await _next(context);
        }
}
