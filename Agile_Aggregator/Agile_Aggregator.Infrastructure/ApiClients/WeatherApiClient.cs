using System.Text.Json;
using Agile_Aggregator.Application.Interfaces;
using Agile_Aggregator.Domain.ValueObjects;
using Agile_Aggregator.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Agile_Aggregator.Infrastructure.ApiClients
{
    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _http;
        private readonly ApiOptions _opts;

        public WeatherApiClient(HttpClient http,
            IOptionsSnapshot<ApiOptions> opts)
        {
            _http = http;
            _opts = opts.Get("Weather");          // named options
            // BaseAddress is already set in Program.cs
        }

        public async Task<WeatherReport> FetchAsync(string city, CancellationToken ct)
        {
            // use _opts.ApiKey, not _apiKey
            var url = $"/data/2.5/weather?q={city}&appid={_opts.ApiKey}";
            var res = await _http.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            using var doc = await JsonDocument.ParseAsync(
                await res.Content.ReadAsStreamAsync(ct),
                cancellationToken: ct);

            var root = doc.RootElement;
            double temp = root.GetProperty("main")
                              .GetProperty("temp").GetDouble();
            int humidity = root.GetProperty("main")
                              .GetProperty("humidity").GetInt32();
            string name = root.GetProperty("name").GetString()!;

            return new WeatherReport(name, temp, humidity);
        }
    }
}

