using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Models;

namespace Agile_Aggregator.Domain.Interfaces
{
    public interface IEndpointFetcher
    {
      //  Task<JsonElement[]> FetchAsync(string name, EndpointSettings endpointSettings, IReadOnlyCollection<Filter> filters, IReadOnlyCollection<Sort> sorts);
        Task<Dictionary<string, JsonElement[]>> FetchAsync(string name, EndpointSettings endpointSettings, IReadOnlyCollection<Filter> filters, IReadOnlyCollection<Sort> sorts);
    }
}
