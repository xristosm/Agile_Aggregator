using Agile_Aggregator.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Agile_Aggregator.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        public CacheService(IMemoryCache cache) => _cache = cache;

        public Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl)
        {
            if (_cache.TryGetValue(key, out T cached))
                return Task.FromResult(cached);

            return AddAndCacheAsync();

            async Task<T> AddAndCacheAsync()
            {
                var item = await factory();
                _cache.Set(key, item, ttl);
                return item;
            }
        }
    }
}