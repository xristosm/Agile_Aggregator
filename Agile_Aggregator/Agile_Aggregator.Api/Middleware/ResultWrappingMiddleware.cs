using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Agile_Aggregator.Api.Middleware
{
    public class ResultWrappingMiddleware
    {
        private readonly RequestDelegate _next;

        public ResultWrappingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            try
            {
                await _next(context);
                memStream.Seek(0, SeekOrigin.Begin);
                var body = await new StreamReader(memStream).ReadToEndAsync();
                context.Response.Body = originalBody;

                if (context.Response.ContentType?.Contains("application/json") == true)
                {
                    var data = JsonSerializer.Deserialize<object>(body);
                    var wrapped = new { IsSuccess = context.Response.StatusCode < 400, Data = data };
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(wrapped);
                }
                else
                {
                    memStream.Seek(0, SeekOrigin.Begin);
                    await memStream.CopyToAsync(originalBody);
                }
            }
            catch (Exception ex)
            {
                context.Response.Body = originalBody;
                context.Response.StatusCode = 500;
                var payload = new { IsSuccess = false, ErrorCode = "UnhandledException", ErrorDetail = ex.Message };
                await context.Response.WriteAsJsonAsync(payload);
            }
        }
    }
}
