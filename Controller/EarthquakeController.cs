using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EarthquakeReporter.Models;
using EarthquakeReporter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace EarthquakeReporter.Controllers
{
    [ApiController]
    [Route("earthquakes")]
    public class EarthquakeController : ControllerBase
    {
        private readonly IEarthquakeDataService _earthquakeDataService;
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly ILogger<EarthquakeController> _logger;

        public EarthquakeController(IEarthquakeDataService earthquakeDataService, ILogger<EarthquakeController> logger, IKafkaProducerService kafkaProducerService)
        {
            _earthquakeDataService = earthquakeDataService;
            _logger = logger;
            _kafkaProducerService = kafkaProducerService;
        }

        [HttpGet("daily")]
        public async Task<IActionResult> GetEarthquakesByDate([FromQuery] DateTime date)
        {
            var startDate = date.Date.ToUniversalTime();
            var endDate = startDate.AddDays(1).AddTicks(-1); // This sets the end time to 23:59:59.9999999 in UTC

            // Log the start and end dates for debugging
            _logger.LogInformation($"Fetching earthquake data for date: {startDate:yyyy-MM-dd}, startDate: {startDate:yyyy-MM-ddTHH:mm:ssZ}, endDate: {endDate:yyyy-MM-ddTHH:mm:ssZ}");

            var earthquakes = await _earthquakeDataService.GetEarthquakesByDateAsync(startDate, endDate);

            if (earthquakes == null || !earthquakes.Any())
            {
                return NotFound($"No earthquake data found for {startDate:yyyy-MM-dd}.");
            }

            var orderedEarthquakes = earthquakes.OrderByDescending(e => DateTime.Parse(e.Date)).ToList();
            return Ok(orderedEarthquakes);
        }
                
        
    }
}
