

using System.Net.Http.Headers;
using Agile_Aggregator.Api.Clients;
using Agile_Aggregator.Application.Factories;
using Agile_Aggregator.Application.Services;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Resilience;
using Agile_Aggregator.Infrastructure.Stores;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<InMemoryStatsStore>();

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// register core and application
builder.Services.AddScoped<IAggregationService, AggregationService>();
builder.Services.AddSingleton<IStatsService, StatsService>();
builder.Services.AddSingleton<ICacheService, CacheService>();

// register factory and clients


// 3) Register all named HttpClients from configuration
builder.Services.AddConfiguredHttpClients(builder.Configuration);

// 4) Register your factory after HttpClientFactory and named clients are available
//    IApiClientFactory depends on IHttpClientFactory, so registration order matters
builder.Services.AddSingleton<IApiClientFactory, ApiClientFactory>();
//builder.Services.AddScoped(sp => sp.GetRequiredService<IApiClientFactory>().CreateClients());

// resilience registry
builder.Services.AddPolicyRegistry(PolicyRegistryBuilder.Build());

/*// optional JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {*//* configure issuer, key, etc. *//*});*/

var app = builder.Build();

// Configure middleware
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agile_Aggregator API V1");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();