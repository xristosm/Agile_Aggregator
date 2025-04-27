using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile_Aggregator.Domain.Filtering;

namespace Agile_Aggregator.Application.QueryStrategies
{
    public abstract class DefaultQueryBuilder : IQueryBuilder
    {
        protected abstract IReadOnlyDictionary<string, string> _map { get; }
        public virtual string Build(
            string baseQuery,
            IReadOnlyCollection<Filter> filters,
            IReadOnlyCollection<Sort> sorts)
        {
            // 1) If nothing was requested, just return the base path unchanged.
            if (!filters.Any() && !sorts.Any())
                return baseQuery;

            var parts = new List<string>();

            // 2) Only add mapped filters
            foreach (var f in filters)
            {
                if (_map.TryGetValue(f.PropertyName, out var mappedKey))
                {
                    parts.Add($"{mappedKey}={Uri.EscapeDataString(f.Value)}");
                }
            }

            // 3) Only add mapped sorts
            if (sorts.Any())
            {
                var sortSegments = sorts
                    .Select(s =>
                    {
                        if (_map.TryGetValue(s.PropertyName, out var mappedKey))
                            return $"{mappedKey},{(s.Descending ? "desc" : "asc")}";
                        return null;
                    })
                    .Where(seg => seg is not null)
                    .ToList();

                if (sortSegments.Any())
                    parts.Add($"sort={string.Join("|", sortSegments)}");
            }

            // 4) If after mapping nothing matched, fall back to baseQuery
            if (!parts.Any())
                return baseQuery;

            // 5) Otherwise assemble into a query string
            return $"{baseQuery}?{string.Join("&", parts)}";
        }


        public  class WeatherQueryBuilder : DefaultQueryBuilder, IQueryBuilder
        {
            protected override IReadOnlyDictionary<string, string> _map { get; }
                = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["city"] = "q",
                    ["country"] = "country",
                    ["units"] = "units",
                    ["lang"] = "lang"
                };
            public override string Build(string baseQuery, IReadOnlyCollection<Filter> filters, IReadOnlyCollection<Sort> sorts)
            {
           
                var query = base.Build(baseQuery, filters, sorts);
                if (query != baseQuery && !query.IsNullOrEmpty())
                    return query;
                return baseQuery;
            }
        }

        public  class NewsQueryBuilder : DefaultQueryBuilder, IQueryBuilder
        {
            protected override IReadOnlyDictionary<string, string> _map { get; }
                   = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                   {
                       ["city"] = "q",
                       ["country"] = "country",
                       ["units"] = "units",
                       ["lang"] = "lang"
                   };
            public override string Build(string baseQuery, IReadOnlyCollection<Filter> filters, IReadOnlyCollection<Sort> sorts)
            {
                var query = base.Build(baseQuery, filters, sorts);
                if (query != baseQuery && !query.IsNullOrEmpty())
                    return query;
                return baseQuery;
            }
        }

        public class UnspesifiedQueryBuilder : IQueryBuilder
        {

            public  string Build(string baseQuery, IReadOnlyCollection<Filter> filters, IReadOnlyCollection<Sort> sorts)
            {

                return baseQuery;
            }
        }



    }

}
