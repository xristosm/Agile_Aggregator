using Agile_Aggregator.Domain.Aggregates;

namespace Agile_Aggregator.Domain.Policies
{
    public interface IAggregationPolicy
    {
        AggregatedInfo Apply(AggregatedInfo data);
    }
}