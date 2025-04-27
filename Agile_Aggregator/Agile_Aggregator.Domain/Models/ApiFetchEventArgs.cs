namespace Agile_Aggregator.Domain.Models
{
    public class ApiFetchEventArgs : EventArgs
    {
        public string ApiName { get; set; } = string.Empty;
        public bool FromCache { get; set; }
        public long ElapsedMs { get; set; }
    }
}
