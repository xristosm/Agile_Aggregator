using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Stores;

namespace Agile_Aggregator.Application.Services
{
    public class EndpointFetcher : IEndpointFetcher
    {


        private readonly IHttpClientFactory _httpFactory;
        private readonly ICacheService _cache;
        private readonly InMemoryStatsStore _store;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);
        public event EventHandler<ApiFetchEventArgs>? FetchCompleted;


        public EndpointFetcher(
                IHttpClientFactory httpFactory,
                ICacheService cache,
                InMemoryStatsStore store
             )
        {
            _store = store;
            _httpFactory = httpFactory;
            _cache = cache;

        }
        public async Task<Result<JsonElement[]>> FetchWithResultAsync(
    string name,
    EndpointSettings endpointSettings,
    IReadOnlyCollection<Filter> filters,
    IReadOnlyCollection<Sort> sorts)
        {
            var stopwatch = Stopwatch.StartNew();
            var cacheKey = $"{name}:{endpointSettings.Query}";
            bool fromCache = true;
            try
            {
                var element = await GetDataWithCacheAsync(
                    cacheKey,
                    name,
                    endpointSettings,
                    CacheTtl,
                    () => fromCache = false);

                var items = ConvertToArray(element);

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
                stopwatch.Stop();
                OnFetchCompleted(name, fromCache, stopwatch.ElapsedMilliseconds);
            }
        }

        private async Task<JsonElement> GetDataWithCacheAsync(
            string cacheKey,
            string clientName,
            EndpointSettings endpointSettings,
            TimeSpan cacheDuration,
            Action markNotFromCache)
        {
            return await _cache.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    markNotFromCache();
                    return await FetchFromHttpAsync(clientName, endpointSettings.Query);
                },
                cacheDuration);
        }

        private async Task<JsonElement> FetchFromHttpAsync(string clientName, string query)
        {
            var client = _httpFactory.CreateClient(clientName);
            using var response = await client.GetAsync(query);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json).RootElement;
        }

        private static JsonElement[] ConvertToArray(JsonElement element)
        {
            return element.ValueKind == JsonValueKind.Array
                ? element.EnumerateArray().ToArray()
                : new[] { element };
        }

        private void OnFetchCompleted(string apiName, bool fromCache, long elapsedMs)
        {
            /*            FetchCompleted?.Invoke(this, new ApiFetchEventArgs
                        {
                            ApiName = apiName,
                            FromCache = fromCache,
                            ElapsedMs = elapsedMs
                        });*/
            var key = fromCache ? $"{apiName}.Cache" : $"{apiName}.Live";
            _store.Add(key, elapsedMs);
        }

  
    }


}

