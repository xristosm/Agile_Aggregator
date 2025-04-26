using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Stores;


namespace Agile_Aggregator.Application.Services
{
    public class StatsService : IStatsService
    {
        private readonly InMemoryStatsStore _store;
        public StatsService(InMemoryStatsStore store) => _store = store;

        public void Record(string apiName, long elapsedMs) => _store.Add(apiName, elapsedMs);

        public IEnumerable<RequestStats> GetAllStats()
            => _store.GetAll()
                     .Select(kv => new RequestStats
                     {
                         ApiName = kv.Key,
                         Count = kv.Value.Count(),
                         AvgMs = kv.Value.Average(),
                         Bucket = kv.Value.Average() < 100 ? "fast"
                                 : kv.Value.Average() < 200 ? "average"
                                                               : "slow"
                     });
    }
}