using Polly;
using Polly.Extensions.Http;

public static class PollyPolicies
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
