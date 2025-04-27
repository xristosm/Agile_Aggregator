
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Agile_Aggregator.API.Controllers;
using Agile_Aggregator.Application.QueryStrategies;
using Agile_Aggregator.Application.Services;
using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class AggregationServiceTests
{
    [Fact]
    public async Task FetchAndAggregateAsync_ReturnsMergedResults()
    {
        // Arrange
        var apiSettings = new ApiSettings
        {
            ["A"] = new EndpointSettings { BaseUrl = string.Empty, Query = "/a" },
            ["B"] = new EndpointSettings { BaseUrl = string.Empty, Query = "/b" }
        };
        var opts = Options.Create(apiSettings);

        var fetcher = new Mock<IEndpointFetcher>();
        var sample = JsonDocument.Parse("[{\"id\":1}]").RootElement;
        fetcher
            .Setup(f => f.FetchWithResultAsync(
                "A", apiSettings["A"],
                It.IsAny<IReadOnlyCollection<Filter>>(),
                It.IsAny<IReadOnlyCollection<Sort>>()))
            .ReturnsAsync(Result<JsonElement[]>.Success(new[] { sample }));
        fetcher
            .Setup(f => f.FetchWithResultAsync(
                "B", apiSettings["B"],
                It.IsAny<IReadOnlyCollection<Filter>>(),
                It.IsAny<IReadOnlyCollection<Sort>>()))
            .ReturnsAsync(Result<JsonElement[]>.Success(new[] { sample }));

        var svc = new AggregationService(opts, fetcher.Object);

        // Act
        var result = await svc.FetchAndAggregateAsync(new List<Filter>(), new List<Sort>());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Data!.ResultsByApi["A"].Data!.Length);
        Assert.Equal(1, result.Data.ResultsByApi["B"].Data!.Length);
    }

    [Fact]
    public async Task FetchAndAggregateAsync_PreservesFailureResult()
    {
        // Arrange
        var apiSettings = new ApiSettings
        {
            ["A"] = new EndpointSettings { BaseUrl = string.Empty, Query = "/a" },
            ["B"] = new EndpointSettings { BaseUrl = string.Empty, Query = "/b" }
        };
        var opts = Options.Create(apiSettings);

        var fetcher = new Mock<IEndpointFetcher>();
        var sample = JsonDocument.Parse("[{\"id\":1}]").RootElement;
        fetcher
            .Setup(f => f.FetchWithResultAsync(
                "A", apiSettings["A"],
                It.IsAny<IReadOnlyCollection<Filter>>(),
                It.IsAny<IReadOnlyCollection<Sort>>()))
            .ReturnsAsync(Result<JsonElement[]>.Failure("HttpError", "Timeout"));
        fetcher
            .Setup(f => f.FetchWithResultAsync(
                "B", apiSettings["B"],
                It.IsAny<IReadOnlyCollection<Filter>>(),
                It.IsAny<IReadOnlyCollection<Sort>>()))
            .ReturnsAsync(Result<JsonElement[]>.Success(new[] { sample }));

        var svc = new AggregationService(opts, fetcher.Object);

        // Act
        var result = await svc.FetchAndAggregateAsync(new List<Filter>(), new List<Sort>());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Data!.ResultsByApi["A"].IsSuccess);
        Assert.Equal("HttpError", result.Data.ResultsByApi["A"].ErrorCode);
        Assert.True(result.Data.ResultsByApi["B"].IsSuccess);
    }



}
/*
// File: Agile_Aggregator.Tests/AggregationServiceTests.cs
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Agile_Aggregator.Application.Services;
using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class AggregationServiceTests
{
    [Fact]
    public async Task FetchAndAggregateAsync_ReturnsMergedResults()
    {
        // Arrange
        var apiSettings = new ApiSettings
        {
            ["A"] = new EndpointSettings { BaseUrl = string.Empty, Query = "/a" },
            ["B"] = new EndpointSettings { BaseUrl = string.Empty, Query = "/b" }
        };
        var opts = Options.Create(apiSettings);

        var fetcher = new Mock<IEndpointFetcher>();
        var sample = JsonDocument.Parse("[{\"id\":1}]").RootElement;
        fetcher
            .Setup(f => f.FetchWithResultAsync(
                "A", apiSettings["A"],
                It.IsAny<IReadOnlyCollection<Filter>>(),
                It.IsAny<IReadOnlyCollection<Sort>>()))
            .ReturnsAsync(Result<JsonElement[]>.Success(new[] { sample }));
        fetcher
            .Setup(f => f.FetchWithResultAsync(
                "B", apiSettings["B"],
                It.IsAny<IReadOnlyCollection<Filter>>(),
                It.IsAny<IReadOnlyCollection<Sort>>()))
            .ReturnsAsync(Result<JsonElement[]>.Success(new[] { sample }));

        var svc = new AggregationService(opts, fetcher.Object);

        // Act
        var result = await svc.FetchAndAggregateAsync(new List<Filter>(), new List<Sort>());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Data!.ResultsByApi["A"].Data!.Length);
        Assert.Equal(1, result.Data.ResultsByApi["B"].Data!.Length);
    }

    [Fact]
    public async Task FetchAndAggregateAsync_PreservesFailureResult()
    {
        // Arrange
        var apiSettings = new ApiSettings
        {
            ["A"] = new EndpointSettings { BaseUrl = string.Empty, Query = "/a" },
            ["B"] = new EndpointSettings { BaseUrl = string.Empty, Query = "/b" }
        };
        var opts = Options.Create(apiSettings);

        var fetcher = new Mock<IEndpointFetcher>();
        var sample = JsonDocument.Parse("[{\"id\":1}]").RootElement;
        fetcher
            .Setup(f => f.FetchWithResultAsync(
                "A", apiSettings["A"],
                It.IsAny<IReadOnlyCollection<Filter>>(),
                It.IsAny<IReadOnlyCollection<Sort>>()))
            .ReturnsAsync(Result<JsonElement[]>.Failure("HttpError", "Timeout"));
        fetcher
            .Setup(f => f.FetchWithResultAsync(
                "B", apiSettings["B"],
                It.IsAny<IReadOnlyCollection<Filter>>(),
                It.IsAny<IReadOnlyCollection<Sort>>()))
            .ReturnsAsync(Result<JsonElement[]>.Success(new[] { sample }));

        var svc = new AggregationService(opts, fetcher.Object);

        // Act
        var result = await svc.FetchAndAggregateAsync(new List<Filter>(), new List<Sort>());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Data!.ResultsByApi["A"].IsSuccess);
        Assert.Equal("HttpError", result.Data.ResultsByApi["A"].ErrorCode);
        Assert.True(result.Data.ResultsByApi["B"].IsSuccess);
    }
}
/*
// File: Agile_Aggregator.Tests/EndpointFetcherTests.cs
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Agile_Aggregator.Application.Services;
using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Infrastructure.Stores;
using Moq;
using Xunit;

public class EndpointFetcherTests
{
    [Fact]
    public async Task FetchWithResultAsync_HttpRequestException_ReturnsFailure()
    {
        // Arrange: simulate client throwing on GetAsync
        var handler = new DelegatingHandlerStub(req => throw new HttpRequestException("Network error"));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://test/") };
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("TestClient")).Returns(client);

        var cache = new Mock<ICacheService>();
        cache
            .Setup(c => c.GetOrAddAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<JsonElement>>>(),
                It.IsAny<TimeSpan>()))
            .ThrowsAsync(new HttpRequestException("Cache failure"));

        var stats = new InMemoryStatsStore();
        var builder = new Func<string, IQueryBuilder>(_ => new DefaultQueryBuilder());
        var fetcher = new EndpointFetcher(factory.Object, cache.Object, stats, builder);

        // Act
        var result = await fetcher.FetchWithResultAsync(
            "TestClient",
            new EndpointSettings { Query = "/test" },
            Array.Empty<Filter>(),
            Array.Empty<Sort>());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("HttpError", result.ErrorCode);
    }
}

// File: Agile_Aggregator.Tests/AuthControllerTests.cs
using System.Threading.Tasks;
using Agile_Aggregator.Api.Controllers;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Api.Models;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var service = new Mock<IAuthService>();
        service
            .Setup(s => s.Authenticate("admin", "password"))
            .ReturnsAsync((true, "jwt-token"));
        var ctrl = new AuthController(service.Object);

        // Act
        var ok = await ctrl.Login(new LoginRequest { Username = "admin", Password = "password" })
            as OkObjectResult;

        // Assert
        Assert.NotNull(ok);
        Assert.Equal(200, ok.StatusCode);
        Assert.Contains("token", ok.Value.ToString());
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var service = new Mock<IAuthService>();
        service.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync((false, string.Empty));
        var ctrl = new AuthController(service.Object);

        // Act
        var res = await ctrl.Login(new LoginRequest { Username = "a", Password = "b" });

        // Assert
        Assert.IsType<UnauthorizedResult>(res);
    }
}

// File: Agile_Aggregator.Tests/StatisticsControllerTests.cs
using System.Collections.Generic;
using Agile_Aggregator.Api.Controllers;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

public class StatisticsControllerTests
{
    [Fact]
    public void GetStats_ReturnsListOfStats()
    {
        // Arrange
        var statsList = new List<ApiStats>
        {
            new ApiStats { ApiName = "X", FastCount = 1, AverageCount = 2, SlowCount = 3, OverallAvgMs = 100 }
        };
        var service = new Mock<IStatsService>();
        service.Setup(s => s.GetStatistics()).Returns(statsList);
        var ctrl = new StatisticsController(service.Object);

        // Act
        var ok = ctrl.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(ok);
        var value = Assert.IsAssignableFrom<IEnumerable<ApiStats>>(ok.Value);
        Assert.Single(value);
    }
}

// File: Agile_Aggregator.Tests/AggregationControllerTests.cs
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Agile_Aggregator.Api.Controllers;
using Agile_Aggregator.Domain.Filtering;
using Agile_Aggregator.Domain.Interfaces;
using Agile_Aggregator.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class AggregationControllerTests
{
    [Fact]
    public async Task Get_ReturnsOkWithData()
    {
        // Arrange
        var json = JsonDocument.Parse("[{\"x\":1}]").RootElement;
        var aggResult = new AggregatedResult
        {
            ResultsByApi = new Dictionary<string, Result<JsonElement[]>>
            {
                ["X"] = Result<JsonElement[]>.Success(new[] { json })
            }
        };
        var service = new Mock<IAggregationService>();
        service.Setup(s => s.FetchAndAggregateAsync(
            It.IsAny<List<Filter>>(),
            It.IsAny<List<Sort>>()))
            .ReturnsAsync(Result<AggregatedResult>.Success(aggResult));
        var ctrl = new AggregationController(service.Object);

        // Act
        var ok = await ctrl.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(ok);
        Assert.Equal(200, ok.StatusCode);
    }
}
*/