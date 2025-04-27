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
            IOptions<ApiSettings> opts,
            IEndpointFetcher fetcher)
        {
            _settings = opts.Value;
            _fetcher = fetcher;
        }

        public async Task<Result<AggregatedResult>> FetchAndAggregateAsync(
            List<Filter>? filters,
            List<Sort>? sorts)
        {
            var work = _settings.Keys
                .Select(name => new
                {
                    name,
                    task = _fetcher.FetchWithResultAsync(
                        name,
                        _settings[name],
                        filters ,
                        sorts )
                })
                .ToList();

            await Task.WhenAll(work.Select(x => x.task));

            var dict = work
                .ToDictionary(
                    x => x.name,
                    x => x.task.Result
                );

            var aggregated = new AggregatedResult { ResultsByApi = dict };
            return Result<AggregatedResult>.Success(aggregated);
        }
    }


