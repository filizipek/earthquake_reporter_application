namespace EarthquakeReporter.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Store hashed password
        public string Province { get; set; }
        public string Country { get; set; }
        public DateTime Birthday { get; set; }
    }
}
