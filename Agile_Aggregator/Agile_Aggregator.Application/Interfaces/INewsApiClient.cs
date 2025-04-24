using Agile_Aggregator.Domain.ValueObjects;

namespace Agile_Aggregator.Application.Interfaces
{
    public interface INewsApiClient
    {
        Task<IReadOnlyList<NewsArticle>> FetchAsync(CancellationToken ct);
    }
}