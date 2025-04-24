using Microsoft.Extensions.Caching.Memory;

public class MemoryCacheDecorator : IExternalApiClient
{
    private readonly IExternalApiClient _inner;
    private readonly IMemoryCache _cache;

    public MemoryCacheDecorator(IExternalApiClient inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public Task<IEnumerable<AggregatedItem>> FetchAsync(AggregationRequestDto req)
    {
        var key = $"{_inner.GetType().Name}:{req.Query}";
        return _cache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return _inner.FetchAsync(req);
        });
    }
}
