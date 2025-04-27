

using System.Net.Http.Headers;
using Agile_Aggregator.Api.BackgroundServices;
using Agile_Aggregator.Api.Extensions;

using Agile_Aggregator.API.Extensions;
using Agile_Aggregator.Application.Factories;
using Agile_Aggregator.Application.QueryStrategies;
using Agile_Aggregator.Application.Services;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Resilience;
using Agile_Aggregator.Infrastructure.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using static Agile_Aggregator.Application.QueryStrategies.DefaultQueryBuilder;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });
    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddJwtAuthentication(builder.Configuration);


builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<InMemoryStatsStore>();

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// register core and application
builder.Services.AddMemoryCache();

builder.Services.AddTransient<ICacheService, CacheService>();
/*builder.Services.AddTransient<ClientEndpointFetcher>();
builder.Services.AddTransient<CachingEndpointFetcher>();
builder.Services.AddTransient<IEndpointFetcherFactory, EndpointFetcherFactory>();*/
builder.Services.AddScoped<IEndpointFetcher, EndpointFetcher>(); 
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddTransient<IAggregationService, AggregationService>();


// register the single BackgroundService
builder.Services.AddHostedService<PerformanceMonitoringService>();

builder.Services.AddExternalApis(builder.Configuration);

builder.Services.AddPolicyRegistry(PolicyRegistryBuilder.Build());

builder.Services.AddSingleton<UnspesifiedQueryBuilder>();
builder.Services.AddSingleton<WeatherQueryBuilder>();
builder.Services.AddSingleton<NewsQueryBuilder>();
// ...etc

// If you need one-per-client:
builder.Services.AddTransient<Func<string, IQueryBuilder>>(sp => key =>
{
    return key switch
    {
        "Weather" => sp.GetRequiredService<WeatherQueryBuilder>(),
        "NewsApi" => sp.GetRequiredService<NewsQueryBuilder>(),
        _ => sp.GetRequiredService<UnspesifiedQueryBuilder>(),
    };
});
var app = builder.Build();

// Configure middleware
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agile_Aggregator API V1");
    //c.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization("ApiScope");

app.Run();