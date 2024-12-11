using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProductPortal.Core.Utilities.Interfaces;
using System.Text;
using System.Text.Json;

namespace ProductPortal.Business.Concrete
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public T Get<T>(string key)
        {
            try
            {
                var value = _cache.GetString(key);

                if (string.IsNullOrEmpty(value))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
                };

                var jsonValue = JsonSerializer.Serialize(value);
                _cache.SetString(key, jsonValue, options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Cache set error for key {key}: {ex.Message}");
            }
        }

        public void Remove(string key) => _cache.Remove(key);
    }
}
