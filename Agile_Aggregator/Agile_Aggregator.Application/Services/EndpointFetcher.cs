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

namespace Agile_Aggregator.Application.Services
{
    public class EndpointFetcher : IEndpointFetcher
    {

            private readonly IHttpClientFactory _httpFactory;
            private readonly ICacheService _cache;

            private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);
            public event EventHandler<ApiFetchEventArgs>? FetchCompleted;
   

        public EndpointFetcher(
                IHttpClientFactory httpFactory,
                ICacheService cache
             )
            {
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
            FetchCompleted?.Invoke(this, new ApiFetchEventArgs
            {
                ApiName = apiName,
                FromCache = fromCache,
                ElapsedMs = elapsedMs
            });
        }

        /*   public async Task<Dictionary<string, JsonElement[]>> FetchAsync(
               string name,
               EndpointSettings endpointSettings,
               IReadOnlyCollection<Filter> filters,
               IReadOnlyCollection<Sort> sorts)
           {
               var cacheKey = $"{name}:{endpointSettings.Query}";
               var stopwatch = Stopwatch.StartNew();

               try
               {
                   var element = await _cache.GetOrAddAsync(
                       cacheKey,
                       async () =>
                       {
                           var response = await _httpFactory
                               .CreateClient(name)
                               .GetStringAsync(endpointSettings.Query);
                           return JsonDocument.Parse(response).RootElement;
                       },
                       CacheTtl);

                   JsonElement[] array;
                   if (element.ValueKind == JsonValueKind.Array)
                       array = element.EnumerateArray().ToArray();
                   else if (element.ValueKind == JsonValueKind.Object &&
                            element.TryGetProperty("items", out var items) &&
                            items.ValueKind == JsonValueKind.Array)
                       array = items.EnumerateArray().ToArray();
                   else
                       array = new[] { element };

                   return new Dictionary<string, JsonElement[]> { { name, array } };
               }
               catch (HttpRequestException)
               {
                   _stats.Record(name + ".HttpError", stopwatch.ElapsedMilliseconds);
                   return new Dictionary<string, JsonElement[]> { { name, Array.Empty<JsonElement>() } };
               }
               catch (JsonException)
               {
                   _stats.Record(name + ".JsonError", stopwatch.ElapsedMilliseconds);
                   return new Dictionary<string, JsonElement[]> { { name, Array.Empty<JsonElement>() } };
               }
               catch (Exception)
               {
                   _stats.Record(name + ".Error", stopwatch.ElapsedMilliseconds);
                   return new Dictionary<string, JsonElement[]> { { name, Array.Empty<JsonElement>() } };
               }
               finally
               {
                   stopwatch.Stop();
                   _stats.Record(name, stopwatch.ElapsedMilliseconds);
               }
           }*/
    }


}

