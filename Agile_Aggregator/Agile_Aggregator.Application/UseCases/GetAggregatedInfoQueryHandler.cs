using Agile_Aggregator.Application.Interfaces;
using Agile_Aggregator.Domain.Aggregates;
using Agile_Aggregator.Domain.Policies;
using Agile_Aggregator.Domain.ValueObjects;

namespace Agile_Aggregator.Application.UseCases
{

        public class GetAggregatedInfoQueryHandler
        {
            private readonly IAggregatedDataFetcher _fetcher;
            private readonly IAggregationPolicy _policy;

            public GetAggregatedInfoQueryHandler(
                IAggregatedDataFetcher fetcher,
                IAggregationPolicy policy)
            {
                _fetcher = fetcher;
                _policy = policy;
            }

            public async Task<AggregatedInfo> HandleAsync(
                GetAggregatedInfoQuery query,
                CancellationToken ct)
            {
                // 1) grab raw data from all three clients in parallel
                var rawData = await _fetcher.FetchAllAsync(ct);

                // 2) build a criteria VO from the query
                var criteria = new AggregationCriteria(
                    NewsKeyword: query.NewsKeyword,
                    SortByStars: query.SortByStars,
                    IncludeWeather: query.IncludeWeather
                );

                // 3) let the policy do the filtering & sorting
                var result = _policy.Apply(rawData, criteria);

                return result;
            }

            public Task<AggregatedInfo> HandleAsync(CancellationToken ct)
            => _fetcher.FetchAllAsync(ct);
    }
}
public class GetAggregatedInfoQuery
{
    // expose whatever filtering/sorting the API supports
    public string? NewsKeyword { get; init; }
    public bool SortByStars { get; init; }
    public bool IncludeWeather { get; init; } = true;
}