public record PerformanceBucket(string Name, TimeSpan Min, TimeSpan Max);

public static class PerformanceBuckets
{
    public static readonly PerformanceBucket Fast = new("Fast", TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
    public static readonly PerformanceBucket Average = new("Average", TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200));
    public static readonly PerformanceBucket Slow = new("Slow", TimeSpan.FromMilliseconds(200), TimeSpan.MaxValue);
}
