using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ProductPortal.Core.Utilities.Results
{
    public class Result
    {
        //Loglama islemleri icin eklendi
        private readonly ILogger<Result> _logger;

        //HTTP context bilgilerine erismek icin eklendi
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Islem sonucu, durum mesajı ve HTTP durum kodunun tutulması için oluşturulmuştur.
        //HTTP durum kodu 200 ,durum mesajı ve başarı durumu true olarak varsayılmıştır.
        public Result(ILogger<Result> logger, IHttpContextAccessor httpContextAccessor, bool success, string message, int statusCode = 200) : this(success)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            Message = message;
            StatusCode = statusCode;
            LogResult();
        }
        public Result(bool success)
        {
            Success = success;
        }
        public bool Success { get; }
        public string Message { get; }

        //HTTP Status Code
        public int StatusCode { get; set; }
        //Error Code
        public int ErrorCode { get; set; }

        //İslemin gerceklestigi zamani tutar
        public DateTime Timestamp { get; } = DateTime.UtcNow;

        //Islem takibi icin eklendi
        public string TraceId { get; } = Guid.NewGuid().ToString();

        //Islem sonucunu loglayan metod.
        private void LogResult()
        {
            //Basarili islemler
            if (Success)
            {
                _logger.LogInformation(
                    "Operation Completed Successfully - TraceId: {TraceId}, Message: {Message}, StatusCode: {StatusCode}, Timestamp: {Timestamp}",
                    TraceId, Message, StatusCode, Timestamp);
            }
            //Basarisiz islemler
            else
            {
                _logger.LogError(
                    "Operation Failed - TraceId: {TraceId}, Error: {Message}, StatusCode: {StatusCode}, ErrorCode: {ErrorCode}, Timestamp: {Timestamp}",
                    TraceId, Message, StatusCode, ErrorCode, Timestamp);
            }
        }
    }
}
