using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Stores;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Agile_Aggregator.Api.BackgroundServices
{
    public class PerformanceMonitoringService : BackgroundService
    {
        // 1) Define two events
        public event Func<CancellationToken, Task> OnSlowRequestCountCheck;
        public event Func<CancellationToken, Task> OnFiveMinuteSpikeCheck;
      

        private readonly InMemoryStatsStore _store;
        private readonly ILogger<PerformanceMonitoringService> _logger;
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

        public PerformanceMonitoringService(
   
            InMemoryStatsStore store,
          
            ILogger<PerformanceMonitoringService> logger)
        {
  
            _store = store;
            _logger = logger;
        



            OnFiveMinuteSpikeCheck += HandleFiveMinuteSpikeAsync;
            OnSlowRequestCountCheck += HandleSlowRequestCountAsync;
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PerformanceMonitoringService starting");
            using var timer = new PeriodicTimer(Interval);
   
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    _logger.LogDebug("Raising spike-check event");
                    if (OnFiveMinuteSpikeCheck != null)
                        await OnFiveMinuteSpikeCheck(stoppingToken);

                    _logger.LogDebug("Raising slow-count-check event");
                    if (OnSlowRequestCountCheck != null)
                        await OnSlowRequestCountCheck(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("PerformanceMonitoringService stopping");
            }
        }

 
        private Task HandleFiveMinuteSpikeAsync(CancellationToken ct)
{
    var now = DateTime.UtcNow;
    const double ThresholdMultiplier = 1.5;
    var recentWindow   = TimeSpan.FromMinutes(5);
    var baselineWindow = TimeSpan.FromHours(1);

    foreach (var kv in _store.GetAll())
    {
        // take a snapshot of all timestamped measurements
        var entries = kv.Value.ToList();
        if (entries.Count == 0)
            continue;

        // 1) Baseline average over the last hour (or fall back to full history)
        var baselineCutoff  = now - baselineWindow;
        var baselineTimes   = entries
            .Where(x => x.TimestampUtc >= baselineCutoff)
            .Select(x => x.ElapsedMs)
            .ToList();

        double baselineAvg = baselineTimes.Count > 0
            ? baselineTimes.Average()
            : entries.Average(x => x.ElapsedMs);

        // 2) Recent average over the last 5 minutes (or fall back to baseline)
        var recentTimes = entries
            .Where(x => x.TimestampUtc >= now - recentWindow)
            .Select(x => x.ElapsedMs)
            .ToList();

        double recentAvg = recentTimes.Count > 0
            ? recentTimes.Average()
            : baselineAvg;

        // 3) Spike check
        if (recentAvg > baselineAvg * ThresholdMultiplier)
        {
            _logger.LogWarning(
                "SPIKE: {Api} 5-min avg {Recent:F0}ms > baseline avg {Baseline:F0}ms × {Mul}",
                kv.Key, recentAvg, baselineAvg, ThresholdMultiplier);
        }
    }

    return Task.CompletedTask;
}


        private Task HandleSlowRequestCountAsync(CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            const int SlowThresholdMs = 200;
            const int CountThreshold = 50;
            var window = TimeSpan.FromMinutes(5);

            foreach (var kv in _store.GetAll())
            {
                var slowCount = kv.Value
                    .Where(e => e.TimestampUtc >= now - window && e.ElapsedMs >= SlowThresholdMs)
                    .Count();

                if (slowCount > CountThreshold)
                {
                    _logger.LogWarning(
                        "HIGH SLOW COUNT: {Api} had {Count} ≥ {Ms}ms in last {Min}m",
                        kv.Key, slowCount, SlowThresholdMs, window.TotalMinutes);
                }
            }

            return Task.CompletedTask;
        }
    }
}
