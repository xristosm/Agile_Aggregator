public interface IExternalApiClient
{
    /// <summary>
    /// Fetch and normalize items for the given request.
    /// </summary>
    Task<IEnumerable<AggregatedItem>> FetchAsync(AggregationRequestDto request);
}
