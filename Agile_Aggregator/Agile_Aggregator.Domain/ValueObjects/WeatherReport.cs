namespace Agile_Aggregator.Domain.ValueObjects
{
    public record WeatherReport
    {
        public string City { get; init; }
        public double TemperatureCelsius { get; init; }
        public int Humidity { get; init; }

        public WeatherReport(string city, double tempKelvin, int humidity)
        {
            City = city ?? throw new ArgumentNullException(nameof(city));
            TemperatureCelsius = tempKelvin - 273.15;
            Humidity = humidity;
        }

        public string ComfortLevel() => TemperatureCelsius switch
        {
            < 5 => "Cold",
            < 25 => "Mild",
            _ => "Hot",
        };
    }
}
