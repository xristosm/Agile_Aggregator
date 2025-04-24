public class AggregationRequestDto
{
    public string Query { get; set; }
    public string SortBy { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
