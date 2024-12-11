using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductPortal.Core.Utilities.Interfaces;

namespace ProductPortal.Core.Utilities.Results
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public T Data { get; }

        public DataResult(T data, bool success) : base(success)
        {
            Data = data;
        }

        public DataResult(ILogger<Result> logger,
                         IHttpContextAccessor httpContextAccessor,
                         T data,
                         bool success,
                         string message,
                         int statusCode = 200)
            : base(logger, httpContextAccessor, success, message, statusCode)
        {
            Data = data;
        }
    }
}
