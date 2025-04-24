using Api_Aggregator.Application.DTOs;

public interface IStatisticsRepository
{
    void Record(string apiName, TimeSpan duration);
    IEnumerable<ApiPerformanceStats> GetAll();
}
