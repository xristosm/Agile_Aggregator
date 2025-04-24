using System.Text.Json;
using Agile_Aggregator.Application.Interfaces;
using Agile_Aggregator.Domain.ValueObjects;

namespace Agile_Aggregator.Infrastructure.ApiClients
{
    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey = "<YOUR_KEY>";

        public WeatherApiClient(HttpClient http) => _http = http;

        public async Task<WeatherReport> FetchAsync(string city, CancellationToken ct)
        {
            var res = await _http.GetAsync($"/data/2.5/weather?q={city}&appid={_apiKey}", ct);
            res.EnsureSuccessStatusCode();
            using var doc = await JsonDocument.ParseAsync(await res.Content.ReadAsStreamAsync(), cancellationToken: ct);
            var root = doc.RootElement;
            double temp = root.GetProperty("main").GetProperty("temp").GetDouble();
            int humidity = root.GetProperty("main").GetProperty("humidity").GetInt32();
            string name = root.GetProperty("name").GetString()!;
            return new WeatherReport(name, temp, humidity);
        }
    }
}
