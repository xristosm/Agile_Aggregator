
using MediatR;

namespace Api_Aggregator.Application.Queries
{ 

    public class AggregateDataQuery : IRequest<IEnumerable<AggregatedItemDto>>
    {

        public AggregationRequestDto Dto { get; }

        public AggregateDataQuery(AggregationRequestDto dto)
        {
            Dto = dto;
        }
    }
}
