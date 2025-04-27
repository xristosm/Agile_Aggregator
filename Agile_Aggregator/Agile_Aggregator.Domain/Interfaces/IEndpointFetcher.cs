using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Models;
using System.Text.Json;

namespace Agile_Aggregator.Domain.Interfaces
{
    public interface IEndpointFetcher
    {
        event EventHandler<ApiFetchEventArgs>? FetchCompleted;

        Task<Result<JsonElement[]>> FetchWithResultAsync(
            string name,
            EndpointSettings settings,
            IReadOnlyCollection<Filter> filters,
            IReadOnlyCollection<Sort> sorts
        );
    }
}