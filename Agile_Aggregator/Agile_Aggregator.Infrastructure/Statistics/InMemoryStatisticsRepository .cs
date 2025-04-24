using System.Collections.Concurrent;
using Api_Aggregator.Application.DTOs;

public class InMemoryStatisticsRepository : IStatisticsRepository
{
    private readonly ConcurrentDictionary<string, List<TimeSpan>> _data
        = new();

    public void Record(string apiName, TimeSpan duration)
    {
        var list = _data.GetOrAdd(apiName, _ => new List<TimeSpan>());
        lock (list) { list.Add(duration); }
    }

    public IEnumerable<ApiPerformanceStats> GetAll() =>
        _data.Select(kvp => {
            var durations = kvp.Value;
            var avg = TimeSpan.FromMilliseconds(
                durations.Average(d => d.TotalMilliseconds));
            var bucket = PerformanceBuckets.Fast;
            if (avg > PerformanceBuckets.Average.Min && avg <= PerformanceBuckets.Average.Max)
                bucket = PerformanceBuckets.Average;
            else if (avg > PerformanceBuckets.Slow.Min)
                bucket = PerformanceBuckets.Slow;

            return new ApiPerformanceStats
            {
                ApiName = kvp.Key,
                TotalRequests = durations.Count,
                AverageTime = avg,
                BucketName = bucket.Name
            };
        });
}
