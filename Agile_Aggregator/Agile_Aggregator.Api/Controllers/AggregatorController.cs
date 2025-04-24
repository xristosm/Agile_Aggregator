using Agile_Aggregator.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AggregatorController : ControllerBase
{
    private readonly GetAggregatedInfoQueryHandler _handler;

    public AggregatorController(GetAggregatedInfoQueryHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(ct);
        return Ok(result);
    }
}