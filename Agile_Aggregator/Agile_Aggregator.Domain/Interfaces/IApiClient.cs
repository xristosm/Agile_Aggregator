using System.Text.Json;
using Agile_Aggregator.Domain.Models;

namespace Agile_Aggregator.Domain.Interfaces
{
    public interface IApiClient
    {
        string Name { get; }
        Task<ApiResponse<IEnumerable<JsonElement>>> FetchAsync(FilterParams filter);
    }
}