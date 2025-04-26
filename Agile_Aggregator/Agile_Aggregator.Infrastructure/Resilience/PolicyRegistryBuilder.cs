using Polly;
using Polly.Registry;

namespace Agile_Aggregator.Infrastructure.Resilience
{
    public static class PolicyRegistryBuilder
    {
        public static PolicyRegistry Build()
        {
            var registry = new PolicyRegistry();
            var retry = Policy.Handle<HttpRequestException>()
                              .WaitAndRetryAsync(3, r => TimeSpan.FromSeconds(Math.Pow(2, r)));
            registry.Add("RetryPolicy", retry);
            return registry;
        }
    }
}
/*using Polly;
using Polly.Extensions.Http;

public static class PolicyRegistryBuilder
{
    public static IAsyncPolicy<HttpResponseMessage> WeatherPolicy =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)));

    public static IAsyncPolicy<HttpResponseMessage> NewsPolicy =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));
}
*/