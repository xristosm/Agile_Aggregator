/*

using System.Net.Http.Headers;
using System.Text.Json;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Clients;

namespace Agile_Aggregator.Infrastructure.Clients
{
    public class GithubApiClient : BaseApiClient, IApiClient
    {
        public string Name => ApiName;
        private const string DefaultOwner = "dotnet";
        private const string DefaultRepo = "runtime";

        public GithubApiClient(HttpClient http, EndpointSettings cfg)
            : base(http, "Github", cfg.ApiKey)
        {

        }

        public async Task<ApiResponse<IEnumerable<JsonElement>>> FetchAsync(string filter)
        {
            string path = $"/repos/{DefaultOwner}/{DefaultRepo}";
            if (!string.IsNullOrWhiteSpace(filter.Owner) &&
                !string.IsNullOrWhiteSpace(filter.Repo))
            {
                // single repo
                path = $"/repos/{filter.Owner}/{filter.Repo}";
            }
            else if (!string.IsNullOrWhiteSpace(filter.Owner))
            {
                // all repos for a user
                path = $"/users/{filter.Owner}/repos";
            }


            var wrapper = await GetJsonAsync<GithubResponse>(filter);
            return new ApiResponse<IEnumerable<JsonElement>> { Data = wrapper.Data ?? [], Source = ApiName };
        }

        private record GithubResponse(IEnumerable<JsonElement> Data);
    }
}


using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Agile_Aggregator.Infrastructure.Clients
{
    public class GithubApiClient : IApiClient
    {
        private readonly ClientSettings _cfg;
        private readonly HttpClient _http;
        public string Name => "Github";

        public GithubApiClient(HttpClient http, ClientSettings cfg)
        {
            _http = http;
            _cfg = cfg;
        }
        public async Task<ApiResponse<IEnumerable<object>>> FetchAsync(FilterParams filter)
        {
            var uri = $"/search/repositories?q={filter.Category ?? ""}";
            var wrapper = await _http.GetFromJsonAsync<GithubResponse>(uri);
            return new ApiResponse<IEnumerable<object>> { Data = wrapper?.Data ?? Enumerable.Empty<object>(), Source = Name };
        }

        private record GithubResponse(IEnumerable<object> Data);
    }
}*/