namespace Agile_Aggregator.Domain.ValueObjects
{
    public record GithubRepository
    {
        public string Name { get; init; }
        public Uri HtmlUrl { get; init; }
        public int Stars { get; init; }

        public GithubRepository(string name, string htmlUrl, int stars)
        {
            Name = !string.IsNullOrWhiteSpace(name)
                ? name
                : throw new ArgumentException("Repo name required", nameof(name));

            HtmlUrl = Uri.TryCreate(htmlUrl, UriKind.Absolute, out var uri)
                ? uri
                : throw new ArgumentException("Invalid URL", nameof(htmlUrl));

            Stars = stars switch
            {
                >= 0 => stars,
                _ => throw new ArgumentOutOfRangeException(nameof(stars), "Stars must be >= 0")
            };
        }

        // Behavior example:
        public bool IsDotNetRepo()
            => Name.Contains("dotnet", StringComparison.OrdinalIgnoreCase);
    }
}
