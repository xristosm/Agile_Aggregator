/*using System.Text.Json;
using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Clients;

namespace Agile_Aggregator.Infrastructure.Clients
{
    public class WeatherClient : BaseApiClient, IApiClient
    {
        public string Name => ApiName;

        public WeatherClient(HttpClient http, EndpointSettings cfg)
            : base(http, "OpenWeather", cfg.ApiKey) { }

        public async Task<ApiResponse<IEnumerable<JsonElement>>> FetchAsync(string query)
        {
            *//*string query = $"q={Uri.EscapeDataString("athens")}";
            if (filter.Latitude.HasValue && filter.Longitude.HasValue)
            {
                query = $"lat={filter.Latitude.Value}&lon={filter.Longitude.Value}";
            }
            else if (!string.IsNullOrWhiteSpace(filter.ZipCode))
            {
                // Escape comma and any other special chars
                query = $"zip={Uri.EscapeDataString(filter.ZipCode!)}";
            }
            else if (!string.IsNullOrWhiteSpace(filter.City))
            {
                query = $"q={Uri.EscapeDataString(filter.City!)}";
            }
  *//*


            // 2️⃣ Build full URI
            var uri = $"data/2.5/weather?{query}&appid={ApiKey}";
            var data = await GetJsonAsync<JsonElement>(uri);
            return new ApiResponse<IEnumerable<JsonElement>> { Data = new[] { data }, Source = ApiName };
        }
    }
}
*/