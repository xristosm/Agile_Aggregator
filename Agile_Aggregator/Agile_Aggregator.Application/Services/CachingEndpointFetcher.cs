/*using System;
using System.Text.Json;
using System.Threading.Tasks;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Domain.Filtering;

namespace Agile_Aggregator.Application.Services
{
    public class CachingEndpointFetcher : IEndpointFetcher
    {
        private readonly IEndpointFetcher _inner;
        private readonly ICacheService _cache;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        public event EventHandler<ApiFetchEventArgs>? FetchCompleted
        {
            add => _inner.FetchCompleted += value;
            remove => _inner.FetchCompleted -= value;
        }

        public CachingEndpointFetcher(ClientEndpointFetcher inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<Result<JsonElement[]>> FetchWithResultAsync(
            string name,
            EndpointSettings settings,
            IReadOnlyCollection<Filter> filters,
            IReadOnlyCollection<Sort> sorts)
        {
            var cacheKey = $"{name}:{settings.Query}";
            return await _cache.GetOrAddAsync(
                cacheKey,
                () => _inner.FetchWithResultAsync(name, settings, filters, sorts),
                CacheTtl);
        }
    }
}
*/