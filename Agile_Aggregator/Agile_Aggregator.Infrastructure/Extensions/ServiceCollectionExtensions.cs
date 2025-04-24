using Microsoft.Extensions.Caching.Memory;        // IMemoryCache
using Microsoft.Extensions.Configuration;         // IConfiguration
using Microsoft.Extensions.DependencyInjection;    // IServiceCollection
using Microsoft.Extensions.Http;

namespace Api_Aggregator.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {
            // 1) Bind your API-key options

            // 2) Memory cache (needed by our decorators)
       

            // 3) Weather client + Polly + caching decorator
            services.AddHttpClient<WeatherApiClient>()
                .AddPolicyHandler(PollyPolicies.WeatherPolicy);
            services.AddSingleton<IExternalApiClient>(sp =>
                new MemoryCacheDecorator(
                    sp.GetRequiredService<WeatherApiClient>(),
                    sp.GetRequiredService<IMemoryCache>()));

            // 4) News client + Polly + caching decorator
            services.AddHttpClient<NewsApiClient>()
                .AddPolicyHandler(PollyPolicies.NewsPolicy);
            services.AddSingleton<IExternalApiClient>(sp =>
                new MemoryCacheDecorator(
                    sp.GetRequiredService<NewsApiClient>(),
                    sp.GetRequiredService<IMemoryCache>()));

            // 5) GitHub client (no key, no caching)
            services.AddHttpClient<GithubApiClient>();
            services.AddSingleton<IExternalApiClient, GithubApiClient>();

            // 6) In-memory statistics repo
            services.AddSingleton<IStatisticsRepository, InMemoryStatisticsRepository>();

            return services;
        }
    }
}
