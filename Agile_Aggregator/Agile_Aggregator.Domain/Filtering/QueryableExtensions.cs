using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Agile_Aggregator.Domain.Filtering
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query, IEnumerable<Filter>? filters)
        {
            if (filters == null) return query;
            foreach (var f in filters)
            {
                var param = Expression.Parameter(typeof(T), "x");
                var prop = Expression.PropertyOrField(param, f.PropertyName);
                var val = Convert.ChangeType(f.Value, prop.Type);
                var constant = Expression.Constant(val);

                Expression body = f.Operator.ToLower() switch
                {
                    "eq" => Expression.Equal(prop, constant),
                    "neq" => Expression.NotEqual(prop, constant),
                    "gt" => Expression.GreaterThan(prop, constant),
                    "gte" => Expression.GreaterThanOrEqual(prop, constant),
                    "lt" => Expression.LessThan(prop, constant),
                    "lte" => Expression.LessThanOrEqual(prop, constant),
                    "contains" => Expression.Call(prop, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, constant),
                    _ => throw new NotSupportedException($"Operator {f.Operator} not supported")
                };

                var lambda = Expression.Lambda<Func<T, bool>>(body, param);
                query = query.Where(lambda);
            }
            return query;
        }

        public static IOrderedQueryable<T> ApplySorting<T>(this IQueryable<T> query, IEnumerable<Sort>? sorts)
        {
            if (sorts == null || !sorts.Any())
                return (query as IOrderedQueryable<T>) ?? query.OrderBy(x => 0);

            IOrderedQueryable<T>? ordered = null;
            bool first = true;
            foreach (var s in sorts)
            {
                var param = Expression.Parameter(typeof(T), "x");
                var prop = Expression.PropertyOrField(param, s.PropertyName);
                var keySelector = Expression.Lambda(prop, param);

                string method = first
                    ? (s.Descending ? "OrderByDescending" : "OrderBy")
                    : (s.Descending ? "ThenByDescending" : "ThenBy");

                var result = Expression.Call(
                    typeof(Queryable), method,
                    new[] { typeof(T), prop.Type },
                    (ordered ?? query).Expression,
                    Expression.Quote(keySelector)
                );

                ordered = (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(result);
                first = false;
            }
            return ordered!;
        }
    }
    public static class StringExtensions
    {
        /// <summary>
        /// Determines whether the specified string is null or an empty string.
        /// </summary>
        public static bool IsNullOrEmpty(this string value)
            => string.IsNullOrEmpty(value);

        /// <summary>
        /// Determines whether the specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string value)
            => string.IsNullOrWhiteSpace(value);
    }
}
