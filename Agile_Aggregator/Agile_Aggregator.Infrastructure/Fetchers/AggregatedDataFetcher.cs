using Agile_Aggregator.Application.Interfaces;
using Agile_Aggregator.Domain.Aggregates;

namespace Agile_Aggregator.Infrastructure.Fetchers
{
    public class AggregatedDataFetcher : IAggregatedDataFetcher
    {
        private readonly IWeatherApiClient _weather;
        private readonly INewsApiClient _news;
        private readonly IGithubApiClient _github;

        public AggregatedDataFetcher(
            IWeatherApiClient weather,
            INewsApiClient news,
            IGithubApiClient github)
        {
            _weather = weather;
            _news = news;
            _github = github;
        }

        public async Task<AggregatedInfo> FetchAllAsync(CancellationToken ct)
        {
            // Parallel fetch
            var wTask = _weather.FetchAsync("London", ct);
            var nTask = _news.FetchAsync(ct);
            var gTask = _github.FetchAsync("dotnet", ct);

            await Task.WhenAll(wTask, nTask, gTask);

            return new AggregatedInfo(
                await wTask,
                await nTask,
                await gTask
            );
        }
    }
}
