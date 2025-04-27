using Agile_Aggregator.Domain.Models;

namespace Agile_Aggregator.Domain.Interfaces
{
    public interface IStatsService
    {

        IEnumerable<ApiBucketStats> GetAllStats();
    }
}