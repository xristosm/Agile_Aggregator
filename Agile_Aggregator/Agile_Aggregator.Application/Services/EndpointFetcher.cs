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
            private readonly IStatsService _stats;
            private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

            public EndpointFetcher(
                IHttpClientFactory httpFactory,
                ICacheService cache,
                IStatsService stats)
            {
                _httpFactory = httpFactory;
                _cache = cache;
                _stats = stats;
            }

            public async Task<Dictionary<string, JsonElement[]>> FetchAsync(
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
            }
        }
        

        /* public async Task<JsonElement[]> FetchAsync(
         string name, EndpointSettings endpointSettings,
             IReadOnlyCollection<Filter> filters,
             IReadOnlyCollection<Sort> sorts)
         {
             // Build the query string
             *//*  var queryString = new ApiRequestBuilder(
                   baseUrl: string.Empty,
                   filterParamMap: endpoint.FilterMap,
                   sortParamMap: endpoint.SortMap)
                   .BuildUrl(filters, sorts);*//*

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

                 // Normalize to array
                 if (element.ValueKind == JsonValueKind.Array)
                     return element.EnumerateArray().ToArray();

                 if (element.ValueKind == JsonValueKind.Object &&
                     element.TryGetProperty("items", out var items) &&
                     items.ValueKind == JsonValueKind.Array)
                 {
                     return items.EnumerateArray().ToArray();
                 }

                 // Fallback: wrap single element
                 return new[] { element };
             }
             catch (HttpRequestException  x)
             {
                 _stats.Record(name + ".HttpError", stopwatch.ElapsedMilliseconds);
                 return Array.Empty<JsonElement>();
             }
             catch (Exception x)
             {
                 _stats.Record(name + ".Error", stopwatch.ElapsedMilliseconds);
                 return Array.Empty<JsonElement>();
             }
             finally
             {
                 stopwatch.Stop();
                 _stats.Record(name, stopwatch.ElapsedMilliseconds);
             }
         }*/
    }

