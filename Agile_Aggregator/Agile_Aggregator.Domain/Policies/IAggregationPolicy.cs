using Agile_Aggregator.Domain.Aggregates;
using Agile_Aggregator.Domain.ValueObjects;

namespace Agile_Aggregator.Domain.Policies
{
    public interface IAggregationPolicy
    {
        AggregatedInfo Apply(AggregatedInfo rawData, AggregationCriteria criteria);
    }
}