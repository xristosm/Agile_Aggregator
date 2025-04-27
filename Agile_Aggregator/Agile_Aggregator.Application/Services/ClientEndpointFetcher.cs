/*using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Agile_Aggregator.Application.Services
{
    public class ClientEndpointFetcher : IEndpointFetcher
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IDistributedCache _cache;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        public event EventHandler<ApiFetchEventArgs>? FetchCompleted;

        public ClientEndpointFetcher(
            IHttpClientFactory httpFactory,
            IDistributedCache cache)
        {
            _httpFactory = httpFactory;
            _cache = cache;
        }

        public async Task<Result<JsonElement[]>> FetchWithResultAsync(
            string name,
            EndpointSettings settings,
            IReadOnlyCollection<Filter> filters,
            IReadOnlyCollection<Sort> sorts)
        {
            var sw = Stopwatch.StartNew();
            var cacheKey = $"{name}:{settings.Query}";

            // 1) Try the cache
            var cachedJson = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedJson))
            {
                var items = JsonSerializer.Deserialize<JsonElement[]>(cachedJson)!;
                sw.Stop();
                FetchCompleted?.Invoke(this, new ApiFetchEventArgs
                {
                    ApiName = name,
                    FromCache = true,
                    ElapsedMs = sw.ElapsedMilliseconds
                });
                return Result<JsonElement[]>.Success(items);
            }

            // 2) Cache miss → fetch from HTTP
            try
            {
                var client = _httpFactory.CreateClient(name);
                var response = await client.GetAsync(settings.Query);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var root = JsonDocument.Parse(json).RootElement;
                var items = root.ValueKind == JsonValueKind.Array
                    ? root.EnumerateArray().ToArray()
                    : new[] { root };

                // 3) Populate the distributed cache
                var blob = JsonSerializer.Serialize(items);
                await _cache.SetStringAsync(
                    cacheKey,
                    blob,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = CacheTtl
                    });

                return Result<JsonElement[]>.Success(items);
            }
            catch (HttpRequestException ex)
            {
                return Result<JsonElement[]>.Failure("HttpError", ex.Message);
            }
            catch (JsonException ex)
            {
                return Result<JsonElement[]>.Failure("ParseError", ex.Message);
            }
            finally
            {
                sw.Stop();
                FetchCompleted?.Invoke(this, new ApiFetchEventArgs
                {
                    ApiName = name,
                    FromCache = false,
                    ElapsedMs = sw.ElapsedMilliseconds
                });
            }
        }
    }
}*/