using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agile_Aggregator.Domain.ValueObjects;

namespace Agile_Aggregator.Domain.Aggregates
{
    public class AggregatedInfo
    {
        public WeatherReport Weather { get; }
        public IReadOnlyList<NewsArticle> News { get; }
        public IReadOnlyList<GithubRepository> Repositories { get; }

        public AggregatedInfo(
            WeatherReport weather,
            IEnumerable<NewsArticle> news,
            IEnumerable<GithubRepository> repos)
        {
            Weather = weather ?? throw new ArgumentNullException(nameof(weather));
            News = news?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(news));
            Repositories = repos?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(repos));
        }

        // Aggregate behavior example:
        public IEnumerable<NewsArticle> TopHeadlines(int max) => News.Take(max);
    }
}

