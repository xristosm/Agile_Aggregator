using System.Text.Json;
using Agile_Aggregator.Application.Interfaces;
using Agile_Aggregator.Domain.ValueObjects;
using Agile_Aggregator.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Agile_Aggregator.Infrastructure.ApiClients
{
    public class NewsApiClient : INewsApiClient
    {
        private readonly HttpClient _http;
        private readonly ApiOptions _opts;

        public NewsApiClient(
            HttpClient http,
            IOptionsMonitor<ApiOptions> optionsMonitor)
        {
            _http = http;
            // “News” must match the name you used in Configure<ApiOptions>(“News”, …)
            _opts = optionsMonitor.Get("News");
        }

        public async Task<IReadOnlyList<NewsArticle>> FetchAsync(CancellationToken ct)
        {
            // build the request URI from your configured options
            var uri = $"v2/top-headlines"
                    + $"?country=us"
                    + $"&apiKey={_opts.ApiKey}";
          
            using var res = await _http.GetAsync(uri, ct);
            res.EnsureSuccessStatusCode();

            using var stream = await res.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            var list = new List<NewsArticle>();
            foreach (var element in doc.RootElement.GetProperty("articles").EnumerateArray())
            {
                var title = element.GetProperty("title").GetString()!;
                var url = element.GetProperty("url").GetString()!;

                // construct your ValueObject directly
                list.Add(new NewsArticle(title, url));
            }

            return list;
        }
    }
}
