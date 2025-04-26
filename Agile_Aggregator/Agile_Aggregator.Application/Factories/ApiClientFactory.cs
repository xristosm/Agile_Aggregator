using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Clients;
using Microsoft.Extensions.Options;


namespace Agile_Aggregator.Application.Factories
{
    public class ApiClientFactory : IApiClientFactory
    {
        private readonly ApiSettings _settings;
        private readonly IHttpClientFactory _httpFactory;

        public ApiClientFactory(
            IOptions<ApiSettings> settings,
            IHttpClientFactory httpFactory)
        {
            _settings = settings.Value;
            _httpFactory = httpFactory;
        }

        public IEnumerable<IApiClient> CreateClients()
        {
            foreach (var kv in _settings)
            {
                var name = kv.Key;
                var ep = kv.Value;
                var client = _httpFactory.CreateClient(name);

                // instantiate correct implementation based on key
                IApiClient apiClient = name switch
                {
                    "Weather" => new WeatherClient(client, ep),
                    "NewsApi" => new NewsApiClient(client, ep),
                    "Github" => new GithubApiClient(client, ep),
                    _ => throw new InvalidOperationException($"Unknown API key '{name}'")
                };

                yield return apiClient;
            }
        }
    }
}
