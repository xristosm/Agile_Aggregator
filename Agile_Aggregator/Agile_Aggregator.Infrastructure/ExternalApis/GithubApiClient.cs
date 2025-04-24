using System.Net.Http.Json;
using System.Text.Json;

public class GithubApiClient : IExternalApiClient
{
    private readonly HttpClient _http;

    public GithubApiClient(HttpClient http) => _http = http;

    public async Task<IEnumerable<AggregatedItem>> FetchAsync(AggregationRequestDto req)
    {
        var url = $"https://api.github.com/search/repositories?q={req.Query}";
        var reqMsg = new HttpRequestMessage(HttpMethod.Get, url);
        reqMsg.Headers.UserAgent.ParseAdd("Api_Aggregator");
        var res = await _http.SendAsync(reqMsg);
        if (!res.IsSuccessStatusCode)
            throw new ExternalApiUnavailableException("GitHub API failed.");

        var doc = await res.Content.ReadFromJsonAsync<JsonDocument>();
        return doc.RootElement
            .GetProperty("items")
            .EnumerateArray()
            .Select(item => new AggregatedItem
            {
                Title = item.GetProperty("full_name").GetString(),
                Source = ApiSource.GitHub,
                PublishedAt = item.GetProperty("created_at").GetDateTime(),
                Url = item.GetProperty("html_url").GetString()
            });
    }
}
