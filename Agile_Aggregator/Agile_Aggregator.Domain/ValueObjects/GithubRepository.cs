namespace Agile_Aggregator.Domain.ValueObjects
{
    public record GithubRepository
    {
        public string Name { get; init; }
        public Uri HtmlUrl { get; init; }

        public GithubRepository(string name, string htmlUrl)
        {
            Name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException("Repo name required", nameof(name))
                : name;
            HtmlUrl = Uri.TryCreate(htmlUrl, UriKind.Absolute, out var uri)
                ? uri
                : throw new ArgumentException("Invalid URL", nameof(htmlUrl));
        }

        // Behavior example:
        public bool IsDotNetRepo() => Name.Contains("dotnet", StringComparison.OrdinalIgnoreCase);
    }
}
