using Agile_Aggregator.Domain.ValueObjects;

namespace Agile_Aggregator.Application.Interfaces
{
    public interface IWeatherApiClient
    {
        Task<WeatherReport> FetchAsync(string city, CancellationToken ct);
    }
}