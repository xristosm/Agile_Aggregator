/*using System;
using Agile_Aggregator.Domain.Interfaces;

namespace Agile_Aggregator.Application.Services
{
    public interface IEndpointFetcherFactory
    {
        IEndpointFetcher GetFetcher(bool useCache);
    }

    public class EndpointFetcherFactory : IEndpointFetcherFactory
    {
        private readonly ClientEndpointFetcher _clientFetcher;
        private readonly CachingEndpointFetcher _cacheFetcher;

        public EndpointFetcherFactory(
            ClientEndpointFetcher clientFetcher,
            CachingEndpointFetcher cacheFetcher)
        {
            _clientFetcher = clientFetcher;
            _cacheFetcher = cacheFetcher;
        }

        public IEndpointFetcher GetFetcher(bool useCache)
            => useCache ? _cacheFetcher : _clientFetcher;
    }
}
*/