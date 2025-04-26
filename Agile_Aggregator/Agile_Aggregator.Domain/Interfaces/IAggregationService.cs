using Agile_Aggregator.Domain.Models;

namespace Agile_Aggregator.Domain.Interfaces
{
    public interface IAggregationService
    {
        Task<AggregatedResult> FetchAndAggregateAsync(FilterParams filter);
    }
}