using Agile_Aggregator.Domain.Filtering;

namespace Agile_Aggregator.Application.QueryStrategies
{
    public interface IQueryBuilder
    {

        string Build(string baseQuery, IReadOnlyCollection<Filter> filters, IReadOnlyCollection<Sort> sorts);
    }
}