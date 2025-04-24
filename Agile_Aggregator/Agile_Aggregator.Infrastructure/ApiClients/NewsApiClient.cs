using System.Text.Json;
using Agile_Aggregator.Application.Interfaces;
using Agile_Aggregator.Domain.ValueObjects;

namespace Agile_Aggregator.Infrastructure.ApiClients
{
    public class NewsApiClient : INewsApiClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey = "<YOUR_KEY>";

        public NewsApiClient(HttpClient http) => _http = http;

        public async Task<IReadOnlyList<NewsArticle>> FetchAsync(CancellationToken ct)
        {
            var res = await _http.GetAsync($"/v2/top-headlines?country=us&apiKey={_apiKey}", ct);
            res.EnsureSuccessStatusCode();
            using var doc = await JsonDocument.ParseAsync(await res.Content.ReadAsStreamAsync(), cancellationToken: ct);
            var articles = doc.RootElement.GetProperty("articles").EnumerateArray();
            var list = new List<NewsArticle>();
            foreach (var a in articles)
            {
                var title = a.GetProperty("title").GetString()!;
                var url = a.GetProperty("url").GetString()!;
                list.Add(new NewsArticle(title, url));
            }
            return list;
        }
    }
}
