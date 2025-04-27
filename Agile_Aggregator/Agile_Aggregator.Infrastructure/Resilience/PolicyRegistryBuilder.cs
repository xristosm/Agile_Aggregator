// File: Infrastructure/Resilience/PolicyRegistryBuilder.cs
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using System;
using System.Net.Http;

namespace Agile_Aggregator.Infrastructure.Resilience
{
    public static class PolicyRegistryBuilder
    {
        public static PolicyRegistry Build()
        {
            var registry = new PolicyRegistry();

            // Retry policy for HttpResponseMessage
            IAsyncPolicy<HttpResponseMessage> retryPolicy =
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        3,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    );

            // Circuit-breaker for HttpResponseMessage
            IAsyncPolicy<HttpResponseMessage> circuitBreakerPolicy =
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 2,
                        durationOfBreak: TimeSpan.FromSeconds(30)
                    );

            registry.Add("RetryPolicy", retryPolicy);
            registry.Add("CircuitBreakerPolicy", circuitBreakerPolicy);

            return registry;
        }
    }
}
