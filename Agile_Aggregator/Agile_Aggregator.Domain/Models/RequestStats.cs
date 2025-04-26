namespace Agile_Aggregator.Domain.Models
{
    public class RequestStats
    {
        public string ApiName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double AvgMs { get; set; }
        public string Bucket { get; set; } = string.Empty;
    }
}
