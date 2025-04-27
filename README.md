# Agile Aggregator API

## Overview

`Agile Aggregator` is a .NET 8 Web API that aggregates data from multiple external REST APIs, applies filtering and sorting, and exposes unified results to clients.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Access to configured external APIs (keys and endpoints)
- Optional: Docker (for containerized deployment)

## Setup & Configuration

1. **Clone the repository**:
   ```bash
   git clone https://github.com/<your‑org>/Agile_Aggregator.git
   cd Agile_Aggregator/Agile_Aggregator.Api
   ```
2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```
3. **Configure API settings**:
   - Open `appsettings.json` (and `appsettings.Development.json` for local/dev).
   - Under `ApiSettings:Keys`, define your API names, URLs, and credentials:
     ```json
     "ApiSettings": {
       "Keys": ["ServiceA", "ServiceB"],
       "ServiceA": {
         "BaseUrl": "https://api.servicea.com/v1/data",
         "ApiKey": "<YOUR_KEY>"
       },
       "ServiceB": { ... }
     }
     ```
4. **Run the application**:
   ```bash
   dotnet run --project Agile_Aggregator.Api
   ```

## API Endpoints

### `GET /api/aggregate`

Aggregates data from all configured external APIs.

- **Query parameters** (all optional):

  - `country` (string)
  - `datetime>=YYYY-MM-DD` (date filter)
  - `date=asc|desc` (sort order)

- **Response** `200 OK`

  ```json
  {
    "resultsByApi": {
      "ServiceA": [ { /* item 1 */ }, { /* item 2 */ }, ... ],
      "ServiceB": [ ... ]
    }
  }
  ```

## Error Handling & Resilience

- Transient failures are handled via **Polly** policies (retry, circuit breaker).
- On persistent failure, a **fallback** returns an empty dataset for the affected API and logs the error.
- All unhandled exceptions are wrapped by `Result<T>` and surface meaningful error codes/messages.

## Unit Testing

- Uses **xUnit** and **Moq** for testing service logic and failure scenarios.
- To run all tests:
  ```bash
  dotnet test --no-build
  ```

## Contribution

1. Fork the repo
2. Create a feature branch (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -m 'Add feature'`)
4. Push to your branch (`git push origin feature/YourFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License. Please see [LICENSE](LICENSE) for details.

