using Agile_Aggregator.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Agile_Aggregator.API.Controllers
{
    [ApiController]
    [Route("api/stats")]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _stats;
        public StatsController(IStatsService stats) => _stats = stats;

        [HttpGet]
        public IActionResult Get() => Ok(_stats.GetAllStats());
    }
}