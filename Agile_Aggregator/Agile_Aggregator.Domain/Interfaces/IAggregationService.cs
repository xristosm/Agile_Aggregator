using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Models;

namespace Agile_Aggregator.Domain.Interfaces
{
    public interface IAggregationService
    {
        Task<Result<AggregatedResult>> FetchAndAggregateAsync(List<Filter>? filters,  List<Sort>? sorts);
    }
}