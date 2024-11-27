using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Utilities.Results
{
    //Hata durumu icin yardımcı sınıf
    public class ErrorResult : Result
    {
        public ErrorResult(ILogger<Result> logger,
                          IHttpContextAccessor httpContextAccessor,
                          string message,
                          int statusCode = 400)
            : base(logger, httpContextAccessor, false, message, statusCode)
        {
        }

        public ErrorResult() : base(false)
        {
        }
    }
}
