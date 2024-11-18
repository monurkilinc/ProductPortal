using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Utilities.Results.SuccessResult
{
    //Basarili bir islem sonucunda ek bir veri dondurmek icin kullanilir
    public class SuccessDataResult<T> : DataResult<T>
    {
        public SuccessDataResult(ILogger<Result> logger,
                              IHttpContextAccessor httpContextAccessor,
                              T data,
                              string message,
                              int statusCode = 200)
           : base(logger, httpContextAccessor, data, true, message, statusCode)
        {
        }

        public SuccessDataResult(ILogger<Result> logger,
                              IHttpContextAccessor httpContextAccessor,
                              T data)
           : base(logger, httpContextAccessor, data, true, "İşlem başarılı", 200)
        {
        }

        public SuccessDataResult(T data) : base(data, true)
        {
        }

        public SuccessDataResult() : base(default, true)
        {
        }
    }
}
