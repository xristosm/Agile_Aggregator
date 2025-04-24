using Api_Aggregator.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("aggregate")]
public class AggregationController : ControllerBase
{
    private readonly IMediator _mediator;
    public AggregationController(IMediator mediator)
        => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] AggregationRequestDto dto)
    {
        var result = await _mediator.Send(new AggregateDataQuery(dto));
        return Ok(result);
    }
}
