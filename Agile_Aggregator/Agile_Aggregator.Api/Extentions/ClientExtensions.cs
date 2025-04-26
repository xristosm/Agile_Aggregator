
using Polly.Extensions.Http;
using Polly;
using Agile_Aggregator.Domain.Models;
using System.Net.Http.Headers;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Infrastructure.Clients;
using Microsoft.Extensions.Options;

namespace Agile_Aggregator.Api.Extentions
{
    public static class HttpClientServiceCollectionExtensions
    {

            private static readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy =
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        3,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                         + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100)),
                        onRetry: (outcome, ts, count, ctx) =>
                            Console.WriteLine($"[Retry {count}] after {ts.TotalSeconds}s")
                    );

            private static readonly IAsyncPolicy<HttpResponseMessage> _circuitBreakerPolicy =
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));

            public static IServiceCollection AddConfiguredHttpClients(
                this IServiceCollection services,
                IConfiguration config)
            {
                // Bind settings
             
            var apiSection = config.GetSection("ApiSettings");

            foreach (var child in apiSection.GetChildren())
            {
                var name = child.Key;
                // Bind the sub-section to your ApiEndpoint model
                var ep = child.Get<EndpointSettings>()!;

                services.AddHttpClient(name, client =>
                {
                    client.BaseAddress = new Uri(ep.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(ep.TimeoutSeconds);
                    if (!string.IsNullOrEmpty(ep.ApiKey))
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", ep.ApiKey);
                    if (!string.IsNullOrEmpty(ep.UserAgent))
                        client.DefaultRequestHeaders.UserAgent.ParseAdd(ep.UserAgent);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(_retryPolicy)
                .AddPolicyHandler(_circuitBreakerPolicy);
            }

            return services;
            }
        }



    /*        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration config)
            {
                // Bind ApiSettings
                var apiSettings = config.GetSection("ApiSettings").Get<ApiSettings>()!;




                // OpenWeather client
                services.AddHttpClient("OpenWeather", c =>
                {
                    c.BaseAddress = new Uri(apiSettings.Weather.BaseUrl);
                    c.Timeout = TimeSpan.FromSeconds(30);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddDefaultPolicies();

                services.AddHttpClient("NewsApi", c =>
                {
                    c.BaseAddress = new Uri(apiSettings.NewsApi.BaseUrl);
                    c.DefaultRequestHeaders.Authorization =
                       new AuthenticationHeaderValue("Bearer", apiSettings.NewsApi.ApiKey);
                    c.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0");
                    c.Timeout = TimeSpan.FromSeconds(30);
                })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddDefaultPolicies();

                services.AddHttpClient("Github", c =>
                {
                    c.BaseAddress = new Uri(apiSettings.GitHub.BaseUrl);
                    c.DefaultRequestHeaders.UserAgent.ParseAdd("MyAppName/1.0");
                    c.Timeout = TimeSpan.FromSeconds(30);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
         .AddPolicyHandler(_circuitBreakerPolicy);


                return services;
            }


            private static readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy =
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        3,
                        attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
                                 + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100)),
                        onRetry: (outcome, ts, count, ctx) =>
                            Console.WriteLine($"[Retry {count}] after {ts.TotalSeconds}s"));

            private static readonly IAsyncPolicy<HttpResponseMessage> _circuitBreakerPolicy =
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));
        }*/
}