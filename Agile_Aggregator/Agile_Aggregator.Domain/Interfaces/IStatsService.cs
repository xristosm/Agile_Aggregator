using Agile_Aggregator.Domain.Models;

namespace Agile_Aggregator.Domain.Interfaces
{
    public interface IStatsService
    {
        void Record(string apiName, long elapsedMs);
        IEnumerable<RequestStats> GetAllStats();
    }
}