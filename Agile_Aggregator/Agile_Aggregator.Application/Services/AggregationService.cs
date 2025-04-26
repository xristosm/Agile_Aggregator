using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using System.Diagnostics;

namespace Agile_Aggregator.Application.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly IApiClientFactory _factory;
        private readonly ICacheService _cache;
        private readonly IStatsService _stats;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        public AggregationService(
          IApiClientFactory factory,
            ICacheService cache,
            IStatsService stats)
        {
            _factory = factory;
            _cache = cache;
            _stats = stats;
        }

        public async Task<AggregatedResult> FetchAndAggregateAsync(FilterParams filter)
        {

            var clients = _factory.CreateClients();
            var tasks = clients.Select(async cl =>
            {
                var key = $"{cl.Name}-{filter.From}-{filter.Category}";
                var sw = Stopwatch.StartNew();

                var response = await _cache.GetOrAddAsync(key,
                    () => cl.FetchAsync(filter), CacheTtl);

                sw.Stop();
                _stats.Record(cl.Name, sw.ElapsedMilliseconds);
                return response.Data;
            });

            var results = await Task.WhenAll(tasks);
            return new AggregatedResult { Items = results.SelectMany(r => r) };
        }
    }
}