using System.Text.Json;

namespace Agile_Aggregator.Domain.Models
{
    public class AggregatedResult
    {
        public Dictionary<string, JsonElement[]> ResultsByApi { get; set; } = new();
    }


}