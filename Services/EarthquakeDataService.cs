using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EarthquakeReporter.Models;

namespace EarthquakeReporter.Services
{

    public interface IEarthquakeDataService{Task<EarthquakeEvent?> GetLatestEarthquakeDataAsync();
    Task<List<EarthquakeEvent>> GetEarthquakesByDateAsync(DateTime startDate, DateTime endDate);
    }
    public class EarthquakeDataService : IEarthquakeDataService
    {
        private readonly IHttpClientService<List<EarthquakeEvent>> _httpClientService;

        public EarthquakeDataService(IHttpClientService<List<EarthquakeEvent>> httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<EarthquakeEvent?> GetLatestEarthquakeDataAsync()
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-5);
            var url = $"https://deprem.afad.gov.tr/apiv2/event/filter?start={startDate:yyyy-MM-ddTHH:mm:ss}&end={endDate:yyyy-MM-ddTHH:mm:ss}";

            try
            {
                var events = await _httpClientService.GetDataAsync(url);
                return events?
                    .Select(e =>
                    {
                        // Try to parse date, return null if parsing fails
                        return DateTime.TryParse(e.Date, out var date) ? new { Event = e, Date = date } : null;
                    })
                    .Where(x => x != null)
                    .OrderByDescending(x => x.Date)
                    .Select(x => x.Event)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Handle or log exception
                // Example: _logger.LogError(ex, "An error occurred while fetching the latest earthquake data.");
                throw; // Rethrow or handle as needed
            }
        }

        public async Task<List<EarthquakeEvent>> GetEarthquakesByDateAsync(DateTime startDate, DateTime endDate)
        {
            var url = $"https://deprem.afad.gov.tr/apiv2/event/filter?start={startDate:yyyy-MM-ddTHH:mm:ss}&end={endDate:yyyy-MM-ddTHH:mm:ss}";

            try
            {
                return await _httpClientService.GetDataAsync(url);
            }
            catch (Exception ex)
            {
                // Handle or log exception
                // Example: _logger.LogError(ex, "An error occurred while fetching earthquake data by date.");
                throw; // Rethrow or handle as needed
            }
        }
    }
}
