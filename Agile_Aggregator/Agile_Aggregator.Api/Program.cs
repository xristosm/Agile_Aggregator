using Agile_Aggregator.Application.Interfaces;
using Agile_Aggregator.Application.UseCases;
using Agile_Aggregator.Infrastructure.ApiClients;
using Agile_Aggregator.Infrastructure.Fetchers;

var builder = WebApplication.CreateBuilder(args);

// --- HttpClient registrations for each API client ---
builder.Services
    .AddHttpClient<IWeatherApiClient, WeatherApiClient>(c =>
        c.BaseAddress = new Uri("https://api.openweathermap.org"))
    .Services
    .AddHttpClient<INewsApiClient, NewsApiClient>(c =>
        c.BaseAddress = new Uri("https://newsapi.org"))
    .Services
    .AddHttpClient<IGithubApiClient, GithubApiClient>(c =>
        c.BaseAddress = new Uri("https://api.github.com"));

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
