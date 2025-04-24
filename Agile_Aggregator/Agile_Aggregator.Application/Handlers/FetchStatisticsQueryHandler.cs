
using Api_Aggregator.Application.DTOs;
using Api_Aggregator.Application.Queries;
using MediatR;

namespace Agile_Aggregator.Application.Handlers
{
    public class FetchStatisticsQueryHandler
        : IRequestHandler<FetchStatisticsQuery, IEnumerable<ApiPerformanceStats>>
    {
        private readonly IStatisticsRepository _stats;
        public FetchStatisticsQueryHandler(IStatisticsRepository stats)
            => _stats = stats;

        public Task<IEnumerable<ApiPerformanceStats>> Handle(
            FetchStatisticsQuery request, CancellationToken ct)
        {
            return Task.FromResult(_stats.GetAll());
        }
    }
}