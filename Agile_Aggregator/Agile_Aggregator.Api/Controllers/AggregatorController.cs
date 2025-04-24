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
    public async Task<IActionResult> Get(
        [FromQuery] string? newsKeyword,
        [FromQuery] bool sortByStars = false,
        [FromQuery] bool includeWeather = true,
        CancellationToken ct = default)
    {
        var query = new GetAggregatedInfoQuery
        {
            NewsKeyword = newsKeyword,
            SortByStars = sortByStars,
            IncludeWeather = includeWeather
        };

        var data = await _handler.HandleAsync(query, ct);
        return Ok(data);
    }

}