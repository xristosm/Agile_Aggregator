
using Api_Aggregator.Application;
using Api_Aggregator.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// bind options
builder.Services.AddMemoryCache();
builder.Services.Configure<WeatherApiOptions>(
    builder.Configuration.GetSection("WeatherApi"));
builder.Services.Configure<NewsApiOptions>(
    builder.Configuration.GetSection("NewsApi"));

 builder.Services.AddInfrastructure(builder.Configuration);

// application & Mediatr
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Marker).Assembly);
});

// controllers, swagger, etc.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error"); // or custom middleware
app.MapControllers();
app.Run();
