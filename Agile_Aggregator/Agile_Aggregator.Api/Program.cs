using Agile_Aggregator.Application.Interfaces;
using Agile_Aggregator.Application.UseCases;
using Agile_Aggregator.Infrastructure.ApiClients;
using Agile_Aggregator.Infrastructure.Configuration;
using Agile_Aggregator.Infrastructure.Fetchers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// --- HttpClient registrations for each API client ---
builder.Services.Configure<ApiOptions>("Weather",
    builder.Configuration.GetSection("Apis:WeatherApi"));
builder.Services.Configure<ApiOptions>("News",
    builder.Configuration.GetSection("Apis:NewsApi"));
builder.Services.Configure<ApiOptions>("GitHub",
    builder.Configuration.GetSection("Apis:GithubApi"));

builder.Services.AddHttpClient<IWeatherApiClient, WeatherApiClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var opts = sp
            .GetRequiredService<IOptionsMonitor<ApiOptions>>()
            .Get("Weather");
        client.BaseAddress = new Uri(opts.BaseUrl);
    });

builder.Services.AddHttpClient<INewsApiClient, NewsApiClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var opts = sp
            .GetRequiredService<IOptionsMonitor<ApiOptions>>()
            .Get("News");
        client.BaseAddress = new Uri(opts.BaseUrl);
    });

builder.Services.AddHttpClient<IGithubApiClient, GithubApiClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var opts = sp
            .GetRequiredService<IOptionsMonitor<ApiOptions>>()
            .Get("GitHub");
        client.BaseAddress = new Uri(opts.BaseUrl);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Agile-Aggregator");
    });

// --- Application & Infrastructure wiring ---
builder.Services.AddScoped<IAggregatedDataFetcher, AggregatedDataFetcher>();
builder.Services.AddScoped<GetAggregatedInfoQueryHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
