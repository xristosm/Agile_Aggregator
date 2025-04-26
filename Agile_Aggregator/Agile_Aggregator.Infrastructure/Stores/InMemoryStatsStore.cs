using System.Collections.Concurrent;

namespace Agile_Aggregator.Infrastructure.Stores
{
    public class InMemoryStatsStore
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<long>> _data = new();
        public void Add(string apiName, long elapsedMs)
        {
            var bag = _data.GetOrAdd(apiName, _ => new ConcurrentBag<long>());
            bag.Add(elapsedMs);
        }
        public IEnumerable<KeyValuePair<string, IEnumerable<long>>> GetAll() =>
            _data.Select(kvp => new KeyValuePair<string, IEnumerable<long>>(kvp.Key, kvp.Value));
    }
}