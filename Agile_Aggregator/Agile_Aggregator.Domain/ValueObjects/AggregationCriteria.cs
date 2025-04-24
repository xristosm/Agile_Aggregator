// Domain/ValueObjects/AggregationCriteria.cs
namespace Agile_Aggregator.Domain.ValueObjects
{
    public record AggregationCriteria(
      string? NewsKeyword = null,
      bool SortByStars = false,
      bool IncludeWeather = true
    );
}
