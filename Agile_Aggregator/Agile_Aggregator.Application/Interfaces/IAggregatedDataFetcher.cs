using Agile_Aggregator.Domain.Aggregates;

namespace Agile_Aggregator.Application.Interfaces
{
    public interface IAggregatedDataFetcher
    {
        Task<AggregatedInfo> FetchAllAsync(CancellationToken ct);
    }
}