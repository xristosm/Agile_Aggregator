using Api_Aggregator.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("stats")]
public class StatisticsController : ControllerBase
{
    private readonly IMediator _mediator;
    public StatisticsController(IMediator mediator)
        => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var stats = await _mediator.Send(new FetchStatisticsQuery());
        return Ok(stats);
    }
}
