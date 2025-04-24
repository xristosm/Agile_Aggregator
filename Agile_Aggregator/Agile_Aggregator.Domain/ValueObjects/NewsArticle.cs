namespace Agile_Aggregator.Domain.ValueObjects
{
    public record NewsArticle
    {
        public string Title { get; init; }
        public Uri Url { get; init; }

        public NewsArticle(string title, string url)
        {
            Title = string.IsNullOrWhiteSpace(title)
                ? throw new ArgumentException("Title cannot be empty", nameof(title))
                : title;
            Url = Uri.TryCreate(url, UriKind.Absolute, out var uri)
                ? uri
                : throw new ArgumentException("Invalid URL", nameof(url));
        }

        // Domain behavior example:
        public bool IsHeadline() => Title.Length < 100;
    }
}
