using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

public class WeatherApiClient : IExternalApiClient
{
    private readonly HttpClient _http;
    private readonly WeatherApiOptions _opts;

    public WeatherApiClient(HttpClient http, IOptions<WeatherApiOptions> opts)
    {
        _http = http;
        _opts = opts.Value;
    }

    public async Task<IEnumerable<AggregatedItem>> FetchAsync(AggregationRequestDto req)
    {
        // Example: use req.Query as city name
        var url = $"{_opts.BaseUrl}/weather?q={req.Query}&appid={_opts.ApiKey}";
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode)
            throw new ExternalApiUnavailableException("Weather API failed.");

        var doc = await res.Content.ReadFromJsonAsync<JsonDocument>();
        // map JSON -> AggregatedItem
        return new[] {
            new AggregatedItem {
                Title = $"Weather in {req.Query}: {doc.RootElement.GetProperty("weather")[0].GetProperty("description").GetString()}",
                Source = ApiSource.Weather,
                PublishedAt = DateTime.UtcNow,
                Url = url
            }
        };
    }
}
    