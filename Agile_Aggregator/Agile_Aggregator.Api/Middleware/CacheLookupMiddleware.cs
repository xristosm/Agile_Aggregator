// Middleware/CacheLookupMiddleware.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Agile_Aggregator.Api.Middleware
{
    public class CacheLookupMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public CacheLookupMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // only check GETs
            if (HttpMethods.IsGet(context.Request.Method))
            {
                var key = $"resp:{context.Request.Path}{context.Request.QueryString}";
                if (_cache.TryGetValue(key, out byte[] cachedBytes))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.ContentLength = cachedBytes.Length;
                    await context.Response.Body.WriteAsync(cachedBytes, 0, cachedBytes.Length);
                    return; // short-circuit pipeline
                }
            }

            // no cached entry → continue down the pipeline
            await _next(context);
        }
    }
}
