namespace EarthquakeReporter.Models
{
    public class EarthquakeEvent
    {
        public string Resource { get; set; } = string.Empty;
        public string EventID { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string Depth { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Magnitude { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public bool IsEventUpdate { get; set; }
        public string? LastUpdateDate { get; set; } = null; // Nullable field
    }
}
