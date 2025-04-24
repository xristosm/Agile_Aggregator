using Agile_Aggregator.Domain.ValueObjects;

namespace Agile_Aggregator.Application.Interfaces
{
    public interface IGithubApiClient
    {
        Task<IReadOnlyList<GithubRepository>> FetchAsync(string user, CancellationToken ct);
    }
}