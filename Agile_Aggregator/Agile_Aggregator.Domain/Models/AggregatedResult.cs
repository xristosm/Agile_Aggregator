using System.Text.Json;

namespace Agile_Aggregator.Domain.Models
{
    public class AggregatedResult
    {
        public IEnumerable<JsonElement> Items { get; set; } = Enumerable.Empty<JsonElement>();
    }
}