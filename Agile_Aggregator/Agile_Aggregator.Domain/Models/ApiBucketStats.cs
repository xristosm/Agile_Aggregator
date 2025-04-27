namespace Agile_Aggregator.Domain.Models
{
    public class ApiBucketStats
    {
        public string ApiName { get; set; } = string.Empty;
        public int FastCount { get; set; }
        public int AverageCount { get; set; }
        public int SlowCount { get; set; }
        public double OverallAvgMs { get; set; }
    }
}