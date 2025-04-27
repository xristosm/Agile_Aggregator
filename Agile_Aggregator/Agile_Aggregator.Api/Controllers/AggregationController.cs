using Agile_Aggregator.Api.Extensions;
using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agile_Aggregator.API.Controllers
{
    [ApiController]
    [Route("api/aggregate")]
    [Authorize(Policy = "ApiScope")]
    public class AggregationController : ControllerBase
    {
        private readonly IAggregationService _aggregator;
        public AggregationController(IAggregationService aggregator) => _aggregator = aggregator;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // e.g. Request.QueryString.Value == "?country=s&datetime>=2021-11-01&date=asc"
            Request.QueryString.Value!.ParseFiltersAndSorts(out var filters, out var sorts);

            //var result = await _aggregator.FetchAndAggregateAsync(filters, sorts);

            var result = await _aggregator.FetchAndAggregateAsync(filters,sorts);
            return Ok(result);
        }
    }
}