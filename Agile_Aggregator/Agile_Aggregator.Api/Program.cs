using Agile_Aggregator.Application.Interfaces;
using Agile_Aggregator.Application.UseCases;
using Agile_Aggregator.Domain.Policies;
using Agile_Aggregator.Infrastructure.ApiClients;
using Agile_Aggregator.Infrastructure.Configuration;
using Agile_Aggregator.Infrastructure.Fetchers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// --- 1) Bind named ApiOptions for each external API ---
builder.Services.Configure<ApiOptions>("Weather",
    builder.Configuration.GetSection("Apis:WeatherApi"));
builder.Services.Configure<ApiOptions>("News",
    builder.Configuration.GetSection("Apis:NewsApi"));
builder.Services.Configure<ApiOptions>("GitHub",
    builder.Configuration.GetSection("Apis:GithubApi"));

// --- 2) Register HttpClients using IOptionsMonitor<T> (singleton-safe) ---
builder.Services.AddHttpClient<IWeatherApiClient, WeatherApiClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var opts = sp.GetRequiredService<IOptionsMonitor<ApiOptions>>().Get("Weather");
        client.BaseAddress = new Uri(opts.BaseUrl);
    });

builder.Services.AddHttpClient<INewsApiClient, NewsApiClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var opts = sp.GetRequiredService<IOptionsMonitor<ApiOptions>>().Get("News");
        client.BaseAddress = new Uri(opts.BaseUrl);
    });

builder.Services.AddHttpClient<IGithubApiClient, GithubApiClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var opts = sp.GetRequiredService<IOptionsMonitor<ApiOptions>>().Get("GitHub");
        client.BaseAddress = new Uri(opts.BaseUrl);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Agile-Aggregator");
    });

// --- 3) DDD wiring: Infrastructure → Application → Domain ---
// Fetcher implements IAggregatedDataFetcher
builder.Services.AddScoped<IAggregatedDataFetcher, AggregatedDataFetcher>();

// Aggregation policy lives in the Domain layer
builder.Services.AddSingleton<IAggregationPolicy, DefaultAggregationPolicy>();

// Use case handler
builder.Services.AddScoped<GetAggregatedInfoQueryHandler>();

// --- 4) MVC + Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
