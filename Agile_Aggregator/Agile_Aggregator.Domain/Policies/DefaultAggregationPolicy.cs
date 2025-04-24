// Domain/Policies/DefaultAggregationPolicy.cs
using Agile_Aggregator.Domain.Aggregates;
using Agile_Aggregator.Domain.ValueObjects;

namespace Agile_Aggregator.Domain.Policies
{
    public class DefaultAggregationPolicy : IAggregationPolicy
    {
        public AggregatedInfo Apply(AggregatedInfo rawData, AggregationCriteria criteria)
        {
            // 1) Filter news by keyword
            var filteredNews = rawData.News
              .Where(n => string.IsNullOrWhiteSpace(criteria.NewsKeyword)
                          || n.Title.Contains(criteria.NewsKeyword, StringComparison.OrdinalIgnoreCase))
              .ToList();

            // 2) Sort GitHub by stars or name
            var sortedRepos = criteria.SortByStars
              ? rawData.Repositories.OrderByDescending(r => r.Stars).ToList()
              : rawData.Repositories.OrderBy(r => r.Name).ToList();

            // 3) Maybe drop weather completely if client asked to skip
            var weather = criteria.IncludeWeather ? rawData.Weather : null;

            return new AggregatedInfo(
              weather,
              filteredNews,
              sortedRepos
            );
        }
    }
}
