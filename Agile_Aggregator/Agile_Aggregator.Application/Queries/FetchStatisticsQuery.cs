using System.Collections.Generic;
using Api_Aggregator.Application.DTOs;
using MediatR;

namespace Api_Aggregator.Application.Queries
{
    /// <summary>
    /// Request to retrieve performance statistics for all external APIs.
    /// </summary>
    public class FetchStatisticsQuery : IRequest<IEnumerable<ApiPerformanceStats>>
    {
        // no additional properties needed
        // the handler will simply pull all data from IStatisticsRepository
    }
}
