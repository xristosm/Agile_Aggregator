using System.Text.Json;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Clients;

namespace Agile_Aggregator.Infrastructure.Clients
{
    public class NewsApiClient : BaseApiClient, IApiClient
    {
        public string Name => ApiName;
        private const string CountryFallback = "gr";
        public NewsApiClient(HttpClient http, EndpointSettings cfg)
            : base(http, "NewsApi", cfg.ApiKey) { }

        public async Task<ApiResponse<IEnumerable<JsonElement>>> FetchAsync(FilterParams filter)
        {

            string path;
            // 1️⃣ Decide endpoint & query
            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                path = $"everything?q={Uri.EscapeDataString(filter.Query)}" +
                       $"&pageSize={filter.PageSize}";
            }
            else
            {
                // top-headlines
                var country = !string.IsNullOrWhiteSpace(filter.Country)
                              ? filter.Country
                              : CountryFallback;

                var parts = new List<string> { $"country={country}", $"pageSize={filter.PageSize}" };
                if (!string.IsNullOrWhiteSpace(filter.Category))
                    parts.Add($"category={Uri.EscapeDataString(filter.Category)}");
                if (!string.IsNullOrWhiteSpace(filter.Sources))
                    parts.Add($"sources={Uri.EscapeDataString(filter.Sources)}");

                path = "top-headlines?" + string.Join("&", parts);
            }
            /*            var category = filter.Category ?? "athens";
                        var uri = $"?q={Uri.EscapeDataString(category)}" +
                                  $"&appid={ApiKey}" +
                                  $"&units=metric"; // or imperial*/
            var wrapper = await GetJsonAsync<NewsResponse>("/v2/"+path);
            return new ApiResponse<IEnumerable<JsonElement>> { Data = wrapper.Articles, Source = ApiName };
        }

        private record NewsResponse(IEnumerable<JsonElement> Articles);
    }
}
/*using Agile_Aggregator.Domain.Models;
using System.Net.Http.Json;

namespace Agile_Aggregator.Infrastructure.Clients
{
    public class NewsApiClient : IApiClient
    {
        private readonly ClientSettings _cfg;
        private readonly HttpClient _http;
        public string Name => "NewsApi";


        public NewsApiClient(HttpClient http, ClientSettings cfg)
        {
            _http = http;
            _cfg = cfg;
        }

        public async Task<ApiResponse<IEnumerable<object>>> FetchAsync(FilterParams filter)
        {
            var uri = $"/everything?q={filter.Category??""}&apiKey={_cfg.ApiKey}";
            var wrapper = await _http.GetFromJsonAsync<NewsResponse>(uri);
            return new ApiResponse<IEnumerable<object>> { Data = wrapper?.Articles ?? Enumerable.Empty<object>(), Source = Name };
        }

        private record NewsResponse(IEnumerable<object> Articles);
    }
}*/