using System.Text.Json;
using Agile_Aggregator.Application.Interfaces;
using Agile_Aggregator.Domain.ValueObjects;

namespace Agile_Aggregator.Infrastructure.ApiClients
{
    public class GithubApiClient : IGithubApiClient
    {
        private readonly HttpClient _http;

        public GithubApiClient(HttpClient http)
        {
            _http = http;
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("Agile-Aggregator");
        }

        public async Task<IReadOnlyList<GithubRepository>> FetchAsync(string user, CancellationToken ct)
        {
            var res = await _http.GetAsync($"/users/{user}/repos", ct);
            res.EnsureSuccessStatusCode();
            using var doc = await JsonDocument.ParseAsync(await res.Content.ReadAsStreamAsync(), cancellationToken: ct);
            var arr = doc.RootElement.EnumerateArray();
            var list = new List<GithubRepository>();
            foreach (var e in arr)
            {
                var name = e.GetProperty("name").GetString()!;
                var url = e.GetProperty("html_url").GetString()!;
                list.Add(new GithubRepository(name, url));
            }
            return list;
        }
    }
}
