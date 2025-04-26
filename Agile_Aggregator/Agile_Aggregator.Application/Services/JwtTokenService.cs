using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class AggregationService : IAggregationService
{
    private readonly ApiSettings _settings;
    private readonly IEndpointFetcher _fetcher;

    public AggregationService(
        IOptions<ApiSettings> settings,
        IEndpointFetcher fetcher)
    {
        _settings = settings.Value;
        _fetcher = fetcher;
    }

    public async Task<AggregatedResult> FetchAndAggregateAsync(
        List<Filter>? filters,
        List<Sort>? sorts)
    {
        // Prepare criteria once
        /*   var filterCriteria = FilterSortMapper.ToFilterCriteria(filters);
           var sortCriteria = FilterSortMapper.ToSortCriteria(sorts);
   */
        // Fetch all endpoints in parallel
        var allResults = new Dictionary<string, JsonElement[]>();
        var totalStopwatch = Stopwatch.StartNew();
    var fetchTasks = _settings
    .Select(kvp =>
    {
        var name    = kvp.Key;
        var cfg     = kvp.Value;
        // you said each cfg.Query already has your  query-string?
        var query   = cfg.Query;

        // e.g. your FetchAsync signature might be:
        // Task<JsonElement[]> FetchAsync(string clientName, string relativeUri, …)
        return _fetcher.FetchAsync(name, cfg, filters ,sorts);
    });

        var endpointResults = await Task.WhenAll(fetchTasks);
        totalStopwatch.Stop();

        // Record total aggregation time
        //_fetcher._stats.Record("AggregationService.Total", totalStopwatch.ElapsedMilliseconds);


        var results = await Task.WhenAll(fetchTasks);
        foreach (var dict in results)
        {
            foreach (var kvp in dict)
            {
                allResults[kvp.Key] = kvp.Value;
            }
        }

        return new AggregatedResult { ResultsByApi = allResults };
     //   return new AggregatedResult { Items = allItems };
    }
}

