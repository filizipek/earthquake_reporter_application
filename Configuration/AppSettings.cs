namespace EarthquakeReporter.Configuration
{
    public class AppSettings
    {
        public SquareUpApiDefinitions SquareUpApiDefinitions { get; set; }
    }

    public class SquareUpApiDefinitions
    {
        public string Domain { get; set; }
        public string Authorization { get; set; }
    }
}



