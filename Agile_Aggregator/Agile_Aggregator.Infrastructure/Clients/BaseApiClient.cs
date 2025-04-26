using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Agile_Aggregator.Domain.Exceptions;
using Agile_Aggregator.Infrastructure.Clients;

namespace Agile_Aggregator.Infrastructure.Clients
{
    /// <summary>
    /// Abstracts HTTP calls, error handling, and JSON parsing for API clients.
    /// </summary>
    public abstract class BaseApiClient
    {
        protected readonly HttpClient Http;
        protected readonly string ApiName;
        protected readonly string ApiKey;

        protected BaseApiClient(HttpClient http, string apiName, string apiKey)
        {
            Http = http;
            ApiName = apiName;
            ApiKey = apiKey;
        }

        protected async Task<T> GetJsonAsync<T>(string uri)
        {
            try
            {
                var response = await Http.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                        var content = await response.Content.ReadAsStringAsync();
                    throw new ApiClientException(ApiName, response.StatusCode, content);
                }

                try
                {
                    return await response.Content.ReadFromJsonAsync<T>()
                        ?? throw new DataParsingException(ApiName, new Exception("No content"));
                }
                catch (JsonException ex)
                {
                    throw new DataParsingException(ApiName, ex);
                }
            }
            catch (TaskCanceledException ex)
            {
                // Timeout
                throw new ApiTimeoutException(ApiName, ex);
            }
            catch (HttpRequestException ex)
            {
                // Connection error
                throw new ApiConnectionException(ApiName, ex);
            }
            catch (ApiClientException ex)
            {
                // Rethrow our own exceptions
                throw ex;
            }
            catch (Exception ex)
            {
                // Unexpected
                throw new ApiClientException(ApiName, "Unexpected error during API call.", ex);
            }
        }
    }
}