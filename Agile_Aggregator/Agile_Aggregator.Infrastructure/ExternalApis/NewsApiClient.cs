using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

public class NewsApiClient : IExternalApiClient
{
    private readonly HttpClient _http;
    private readonly NewsApiOptions _opts;

    public NewsApiClient(HttpClient http, IOptions<NewsApiOptions> opts)
    {
        _http = http;
        _opts = opts.Value;
    }

    public async Task<IEnumerable<AggregatedItem>> FetchAsync(AggregationRequestDto req)
    {
        var url = $"{_opts.BaseUrl}/everything?q={req.Query}&apiKey={_opts.ApiKey}";
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode)
            throw new ExternalApiUnavailableException("News API failed.");

        var wrapper = await res.Content.ReadFromJsonAsync<NewsApiResponse>();
        return wrapper.Articles.Select(a => new AggregatedItem
        {
            Title = a.Title,
            Source = ApiSource.News,
            PublishedAt = a.PublishedAt,
            Url = a.Url
        });
    }
}
public class NewsApiResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }

    [JsonPropertyName("articles")]
    public List<NewsArticle> Articles { get; set; }
}

/// <summary>
/// Single article record from NewsAPI.
/// </summary>
public class NewsArticle
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("publishedAt")]
    public DateTime PublishedAt { get; set; }

    // You can add more fields here if you need them, e.g.:
    // [JsonPropertyName("description")]
    // public string Description { get; set; }
}