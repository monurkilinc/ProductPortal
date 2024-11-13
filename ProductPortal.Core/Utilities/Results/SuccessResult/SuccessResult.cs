using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Utilities.Results.SuccessResult
{
    //Basari durumu icin yardımcı sınıf
    public class SuccessResult : Result
    {
        public SuccessResult(ILogger<Result> logger,
                           IHttpContextAccessor httpContextAccessor,
                           string message,
                           int statusCode = 200)
            : base(logger, httpContextAccessor, true, message, statusCode)
        {
        }

        public SuccessResult() : base(true)
        {
        }
    }
}
