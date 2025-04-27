using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Resilience;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Agile_Aggregator.Api.Extensions
{
    public static class HttpClientServiceCollectionExtensions
    {
        public static IServiceCollection AddExternalApis(
            this IServiceCollection services,
            IConfiguration config)
        {
            var settings = config.GetSection("ApiSettings")
                                 .Get<Dictionary<string, EndpointSettings>>();

            var registry = PolicyRegistryBuilder.Build();
            services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);

            foreach (var kv in settings)
            {
                var name = kv.Key;
                var ep = kv.Value;

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
                .AddPolicyHandlerFromRegistry("RetryPolicy")
                .AddPolicyHandlerFromRegistry("CircuitBreakerPolicy")
                .AddPolicyHandler(
                    Policy<HttpResponseMessage>
                        .HandleResult(r => !r.IsSuccessStatusCode)
                        .FallbackAsync(
                            fallbackAction: (_, __) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent("\"CACHE_FALLBACK\"")
                            }),
                            onFallbackAsync: (_, __) => Task.CompletedTask
                        )
                );
            }

            return services;
        }
    }
}
/*
using Polly.Extensions.Http;
using Polly;
using Agile_Aggregator.Domain.Models;
using System.Net.Http.Headers;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Infrastructure.Clients;
using Microsoft.Extensions.Options;
using Agile_Aggregator.Infrastructure.Resilience;
using System.Net;

namespace Agile_Aggregator.Api.Extentions
{
    public static class HttpClientServiceCollectionExtensions
    {


        public static IServiceCollection AddExternalApis(this IServiceCollection services, IConfiguration config)
        {
            var settings = config.GetSection("ApiSettings").Get<ApiSettings>();

            // build and register your policy registry
            var registry = PolicyRegistryBuilder.Build();
            services.AddSingleton(registry);

            foreach (var name in settings.Keys)
            {
                var ep = settings[name];
                services.AddHttpClient(name, client =>
                {
                    client.BaseAddress = new Uri(ep.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(ep.TimeoutSeconds);
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(ep.UserAgent);
                    if (!string.IsNullOrEmpty(ep.ApiKey))
                        client.DefaultRequestHeaders.Add("X-API-Key", ep.ApiKey);
                })
                .AddPolicyHandlerFromRegistry("RetryPolicy")
                .AddPolicyHandlerFromRegistry("CircuitBreakerPolicy")
                .AddPolicyHandler(Policy<HttpResponseMessage>
                    .HandleResult(r => !r.IsSuccessStatusCode)
                    .FallbackAsync(
                        fallbackAction: async (ct, ctx) => new HttpResponseMessage(HttpStatusCoade.OK)
                        {
                            Content = new StringContent("CACHE_FALLBACK")
                        },
                        onFallbackAsync: (outcome, ctx) =>
                        {
                            // you can log here that you’re falling back
                            return Task.CompletedTask;
                        }
                    ));
            }

            return services;
        }
    }
}
*/

/*    
 *                private static readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy =
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
 *    
 *    
 *    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration config)
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
