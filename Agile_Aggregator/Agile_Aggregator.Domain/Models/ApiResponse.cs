namespace Agile_Aggregator.Domain.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; } = default!;
        public string Source { get; set; } = string.Empty;
    }
}