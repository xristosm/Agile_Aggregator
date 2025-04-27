using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Agile_Aggregator.Infrastructure.Stores
{
    public class InMemoryStatsStore
    {
        // now stores a timestamp + elapsedMs
        private readonly ConcurrentDictionary<string, ConcurrentBag<(DateTime TimestampUtc, long ElapsedMs)>> _data
            = new();

        /// <summary>
        /// Record one call for the given apiName, stamping it with UtcNow.
        /// </summary>
        public void Add(string apiName, long elapsedMs)
        {
            var bag = _data.GetOrAdd(
                apiName,
                _ => new ConcurrentBag<(DateTime, long)>()
            );

            // capture the moment we added it
            bag.Add((DateTime.UtcNow, elapsedMs));
        }

        /// <summary>
        /// Returns each API’s full history of (timestamp, elapsedMs) tuples.
        /// </summary>
        public IEnumerable<KeyValuePair<string, IEnumerable<(DateTime TimestampUtc, long ElapsedMs)>>> GetAll() =>
            _data.Select(kvp =>
                new KeyValuePair<string, IEnumerable<(DateTime, long)>>(
                    kvp.Key,
                    // Make a snapshot list so callers can .ToList() / LINQ safely
                    kvp.Value.ToList()
                )
            );
    }
}
