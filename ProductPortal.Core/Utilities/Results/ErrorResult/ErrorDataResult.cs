using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Utilities.Results.ErrorResult
{
    public class ErrorDataResult<T> : DataResult<T>
    {
        public ErrorDataResult(ILogger<Result> logger,
                             IHttpContextAccessor httpContextAccessor,
                             T data,
                             string message,
                             int statusCode = 400)
            : base(logger, httpContextAccessor, data, false, message, statusCode)
        {
        }

        public ErrorDataResult(ILogger<Result> logger,
                             IHttpContextAccessor httpContextAccessor,
                             string message)
            : base(logger, httpContextAccessor, default, false, message, 400)
        {
        }

        public ErrorDataResult() : base(default, false)
        {
        }
    }
}
