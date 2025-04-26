using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agile_Aggregator.API.Controllers
{
    [ApiController]
    [Route("api/aggregate")]
    //[Authorize]
    public class AggregationController : ControllerBase
    {
        private readonly IAggregationService _aggregator;
        public AggregationController(IAggregationService aggregator) => _aggregator = aggregator;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] FilterParams filter)
        {
            var result = await _aggregator.FetchAndAggregateAsync(filter);
            return Ok(result);
        }
    }
}