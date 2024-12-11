using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductPortal.Core.Middleware;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Exceptions
{
    public class GlobalExceptionHandler : ExceptionHandlingMiddleware
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    : base(next)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {

            var response = new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "İşlem sırasında bir hata oluştu"
            };

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}
