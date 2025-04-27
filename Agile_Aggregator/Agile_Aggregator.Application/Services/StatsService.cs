using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Stores;


namespace Agile_Aggregator.Application.Services
{
    public class StatsService : IStatsService
    {
        private readonly InMemoryStatsStore _store;

        public StatsService(InMemoryStatsStore store)// IEndpointFetcher fetcher)
        {
            _store = store;

        }

        public IEnumerable<ApiBucketStats> GetAllStats()
            => _store.GetAll().Select(kv =>
            {

                var timings = kv.Value.ToList();
                return new ApiBucketStats
                {
                    ApiName = kv.Key,
                    FastCount = timings.Count(t => t.ElapsedMs < 100),
                    AverageCount = timings.Count(t => t.ElapsedMs >= 100 && t.ElapsedMs < 200),
                    SlowCount = timings.Count(t => t.ElapsedMs >= 200),
                    OverallAvgMs = timings.Any() ? timings.Select(i=>i.ElapsedMs).Average() : 0
                };
            });
    }
}