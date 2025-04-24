namespace Api_Aggregator.Application.DTOs
{
    /// <summary>
    /// Performance statistics for a single external API.
    /// </summary>
    public class ApiPerformanceStats
    {
        /// <summary>
        /// Name of the API (e.g. "WeatherApiClient").
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Total number of requests made.
        /// </summary>
        public int TotalRequests { get; set; }

        /// <summary>
        /// Average response time across all requests.
        /// </summary>
        public TimeSpan AverageTime { get; set; }

        /// <summary>
        /// Bucket name for average time (e.g. "Fast", "Average", "Slow").
        /// </summary>
        public string BucketName { get; set; }
    }
}
