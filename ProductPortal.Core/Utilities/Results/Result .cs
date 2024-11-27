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
    public abstract class Result :IResult
    {
        protected readonly ILogger<Result> _logger;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        //Islem sonucu, durum mesajı ve HTTP durum kodunun tutulması için oluşturulmuştur.
        //HTTP durum kodu 200 ,durum mesajı ve başarı durumu true olarak varsayılmıştır.
        public Result(ILogger<Result> logger, IHttpContextAccessor httpContextAccessor, bool success, string message, int statusCode = 200) : this(success)
        {
            Success = success;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            Message = message;
            StatusCode = statusCode;
            LogResult();
        }
        protected Result(bool success)
        {
            Success = success;
        }
        public bool Success { get; }
        public string Message { get; }
        public int StatusCode { get; set; }
        public int ErrorCode { get; set; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;
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

        public virtual async Task ExecuteAsync(HttpContext httpContext)
        {
            try
            {
                _logger.LogInformation($"Processing request: {httpContext.Request.Path}");

                // İsteği bir sonraki middleware'e ilet
                await httpContext.Response.WriteAsync("İşlem tamamlandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request processing error");
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}
