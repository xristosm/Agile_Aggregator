using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Resilience;
using Microsoft.AspNetCore.WebUtilities;
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
                .AddPolicyHandlerFromRegistry("CircuitBreakerPolicy");
                 /*    .AddPolicyHandler(
                    Policy<HttpResponseMessage>
                        .HandleResult(r => !r.IsSuccessStatusCode)
                   .FallbackAsync(
                            fallbackAction: (_, __) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent("no Data Could be Colected")
                            }),
                            onFallbackAsync: (_, __) => Task.CompletedTask
                      
                );  )*/
            }

            return services;
        }
    }
    public static class QueryParsingExtensions
    {
        // list of supported filter operators
        private static readonly string[] _operators = new[] { ">=", "<=", "!=", ">", "<" };

        /// <summary>
        /// Parses a raw query string (e.g. "?country=s&datetime>=2021-11-01&date=asc")
        /// into a list of Filters and a list of Sorts.
        /// </summary>
        public static void ParseFiltersAndSorts(
            this string? rawQueryString,
            out List<Filter> filters,
            out List<Sort> sorts)
        {
            filters = new List<Filter>();
            sorts = new List<Sort>();

            if (string.IsNullOrWhiteSpace(rawQueryString))
                return;

            // strip leading '?' if present
            var qs = rawQueryString.StartsWith("?", StringComparison.Ordinal)
                ? rawQueryString[1..]
                : rawQueryString;

            // parse into key -> StringValues
            var dict = QueryHelpers.ParseQuery(qs);

            foreach (var kv in dict)
            {
                var key = kv.Key;
                var values = kv.Value;
                foreach (var rawVal in values)
                {
                    // if value is "asc" or "desc", treat as a sort
                    if (string.Equals(rawVal, "asc", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(rawVal, "desc", StringComparison.OrdinalIgnoreCase))
                    {
                        sorts.Add(new Sort
                        {
                            PropertyName = key,
                            Descending = rawVal.Equals("desc", StringComparison.OrdinalIgnoreCase)
                        });
                        continue;
                    }

                    // otherwise, parse as a filter: look for operator in key
                    var op = "=";
                    var name = key;
                    foreach (var symbol in _operators)
                    {
                        var idx = key.IndexOf(symbol, StringComparison.Ordinal);
                        if (idx >= 0)
                        {
                            name = key[..idx];
                            op = symbol;
                            break;
                        }
                    }

                    filters.Add(new Filter
                    {
                        PropertyName = name,
                        Operator = op,
                        Value = rawVal
                    });
                }
            }
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
