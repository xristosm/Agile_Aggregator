namespace Agile_Aggregator.Domain.Models
{
    public class ApiSettings : Dictionary<string, EndpointSettings>
    { }
    public class EndpointSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
        public string UserAgent { get; set; } = string.Empty;
    }
}



