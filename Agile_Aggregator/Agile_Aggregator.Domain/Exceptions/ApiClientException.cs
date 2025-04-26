using System;
using System.Net;

namespace Agile_Aggregator.Domain.Exceptions

{
    /// <summary>
    /// Base exception for all API client errors.
    /// </summary>
    public class ApiClientException : Exception
    {
        public string ApiName { get; }
        public HttpStatusCode? StatusCode { get; }

        public ApiClientException(string apiName, string message, Exception? inner = null)
            : base(message, inner)
        {
            ApiName = apiName;
        }

        public ApiClientException(string apiName, HttpStatusCode status, string content)
            : base($"{apiName} returned {(int)status} with content: {content}")
        {
            ApiName = apiName;
            StatusCode = status;
        }
    }

    /// <summary>
    /// Thrown when the client fails to parse the response JSON.
    /// </summary>
    public class DataParsingException : ApiClientException
    {
        public DataParsingException(string apiName, Exception inner)
            : base(apiName, "Error parsing data from API.", inner) { }
    }

    /// <summary>
    /// Thrown when a request times out.
    /// </summary>
    public class ApiTimeoutException : ApiClientException
    {
        public ApiTimeoutException(string apiName, Exception inner)
            : base(apiName, "Request to API timed out.", inner) { }
    }

    /// <summary>
    /// Thrown when there is a connection issue.
    /// </summary>
    public class ApiConnectionException : ApiClientException
    {
        public ApiConnectionException(string apiName, Exception inner)
            : base(apiName, "Connection error while calling API.", inner) { }
    }
}