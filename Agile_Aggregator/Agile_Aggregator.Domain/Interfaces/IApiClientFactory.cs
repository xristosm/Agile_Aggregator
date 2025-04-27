using Agile_Aggregator.Domain.Interfaces;

namespace Agile_Aggregator.Domain.Interfaces
{
    public interface IApiClientFactory
    {
       // IEnumerable<IApiClient> CreateClients();
        Task<T> FetchDataAsync<T>(string relativeUri);
    }
}