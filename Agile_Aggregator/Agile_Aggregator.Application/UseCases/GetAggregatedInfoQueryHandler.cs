using Agile_Aggregator.Application.Interfaces;
using Agile_Aggregator.Domain.Aggregates;

namespace Agile_Aggregator.Application.UseCases
{
    public class GetAggregatedInfoQueryHandler
    {
        private readonly IAggregatedDataFetcher _fetcher;

        public GetAggregatedInfoQueryHandler(IAggregatedDataFetcher fetcher)
            => _fetcher = fetcher;

        public Task<AggregatedInfo> HandleAsync(CancellationToken ct)
            => _fetcher.FetchAllAsync(ct);
    }
}
