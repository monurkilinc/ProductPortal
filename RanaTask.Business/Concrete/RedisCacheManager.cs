using Microsoft.Extensions.Caching.Distributed;
using ProductPortal.Core.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductPortal.Business.Concrete
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        public RedisCacheService(IDistributedCache cache) => _cache = cache;

        public T Get<T>(string key)
        {
            var value = _cache.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
            };
            _cache.SetString(key, JsonSerializer.Serialize(value), options);
        }

        public void Remove(string key) => _cache.Remove(key);
    }
}
