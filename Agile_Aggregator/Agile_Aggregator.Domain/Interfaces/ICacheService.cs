namespace Agile_Aggregator.Domain.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl);
    }
}