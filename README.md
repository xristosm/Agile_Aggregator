# Agile Aggregator API

## Overview

`Agile Aggregator` is a .NET 8 Web API that fetches, filters, sorts, and unifies data from multiple external REST APIs into a single aggregated response.

It provides:

- **Authentication** via `POST /api/auth/login` (use `username=admin`, `password=password`).
- **Aggregation** via `GET /api/aggregate` with dynamic filtering and sorting.
- **Statistics** via `GET /api/statistics` to view runtime metrics and fetch performance.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Valid external API keys/endpoints configured in `appsettings.json`
- Optional: Docker for containerization

---

## Setup & Configuration

1. **Clone the repository**

   ```bash
   git clone https://github.com/<org>/Agile_Aggregator.git
   cd Agile_Aggregator/Agile_Aggregator.Api
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Configure API settings**

   Add any number of external APIs under the `ApiSettings` section in `appsettings.json`. Example:

   ```json
   "ApiSettings": {
     "Weather": {
       "BaseUrl": "https://api.openweathermap.org",
       "ApiKey": "027cc235754c563ba5e78056c45657d9",
       "Query": "/data/2.5/weather?q=athens&appid=027cc235754c563ba5e78056c45657d9"
     },
     "NewsApi": {
       "BaseUrl": "https://newsapi.org",
       "ApiKey": "594fc9d848a04f59b1a55ce3ace21071",
       "Query": "/v2/top-headlines?country=us",
       "UserAgent": "MyApp/1.0"
     },
     "Github": {
       "BaseUrl": "https://api.github.com",
       "UserAgent": "MyApp/1.0",
       "Query": "/repos/dotnet/runtime"
     }
   }
   ```

4. **Run the application**

   ```bash
   dotnet run --project Agile_Aggregator.Api
   ```

---

## Authentication

### `POST /api/auth/login`

Logs in with credentials and returns a JWT bearer token.

- **Request Body** (JSON):
  ```json
  {
    "username": "admin",
    "password": "password"
  }
  ```
- **Response** `200 OK`:
  ```json
  {
    "token": "<JWT_TOKEN>"
  }
  ```

Include `Authorization: Bearer <JWT_TOKEN>` on subsequent calls.

---

## API Endpoints

### `GET /api/aggregate`

Fetches and merges data from all configured external APIs. For each API name (as defined in `ApiSettings`), calls the external endpoint, applies any filters/sorts, and returns per-API results wrapped in a standard `Result<T>`.

- **Query Parameters** (optional, can repeat):

  - **Filters**: `name=value` (e.g. `country=us`)
  - **Sorts**: `sort=asc` or `sort=desc`

  The service parses these parameters and attempts to map them into each API’s own query schema before aggregation.

- **Response** `200 OK`:

  ```json
  {
    "isSuccess": true,
    "data": {
      "resultsByApi": {
        "Weather": [ /* weather JSON elements */ ],
        "NewsApi": [ /* news items */ ],
        "Github": [ /* repo info JSON */ ]
      }
    }
  }
  ```

- **Result**** Wrapper**:

  ```csharp
  public class Result<T> {
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? ErrorCode { get; }
    public string? ErrorDetail { get; }
    // static methods Success(T) and Failure(code, detail)
  }
  ```

### `GET /api/statistics`

Returns performance metrics for each configured API fetch, including counts of fast/average/slow responses and overall averages.

- **Response** `200 OK`:
  ```json
  [
    {
      "apiName": "Weather",
      "fastCount": 10,
      "averageCount": 5,
      "slowCount": 2,
      "overallAvgMs": 180.5
    },
    {
      "apiName": "NewsApi",
      "fastCount": 8,
      "averageCount": 6,
      "slowCount": 1,
      "overallAvgMs": 200.0
    }
  ]
  ```
  Backing model:
  ```csharp
  public class ApiStats {
    public string ApiName { get; set; } = string.Empty;
    public int FastCount { get; set; }
    public int AverageCount { get; set; }
    public int SlowCount { get; set; }
    public double OverallAvgMs { get; set; }
  }
  ```

---

## Error Handling & Resilience

- Uses **Polly** for retries and circuit breakers on transient HTTP failures.
- On sustained failures, the aggregate result contains a failed `Result<T>` for that API with error codes (`HttpError`, `ParseError`, etc.).
- Successful responses for each API are cached (default TTL = 5 minutes).

---

## Swagger / OpenAPI

Swagger UI is available at `http://localhost:5000/swagger/index.html`.

Ensure in `Program.cs` you have:

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
  options.SwaggerDoc("v1", new() { Title = "Agile Aggregator API", Version = "v1" });
  var xmlFile = "Agile_Aggregator.Api.xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

Enable XML doc generation in `.csproj`:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

---

## Unit Testing

- Tests written with **xUnit** and **Moq**.
- Run all tests:
  ```bash
  dotnet test --no-build
  ```

---

## Contributing

1. Fork the repo
2. Create a branch: `git checkout -b feature/YourFeature`
3. Commit: `git commit -m "Add feature"`
4. Push: `git push origin feature/YourFeature`
5. Open a PR for review

---



