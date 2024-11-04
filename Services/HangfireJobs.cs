using EarthquakeReporter.Models;
using EarthquakeReporter.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

public interface IHangfireJobs
{
    Task FetchAndLogLatestEarthquakeData();
    Task FetchAndLogDailyEarthquakeData(DateTime date); 
    Task FetchAndLogDailyEarthquakeDataWithMagnitude(DateTime date, double magnitude);
}

public class HangfireJobs : IHangfireJobs
{
    private readonly IEarthquakeDataService _earthquakeDataService;
    private readonly IKafkaProducerService _kafkaProducerService;
    //private readonly IEmailService _emailService; // Commented out for now
    private readonly ILogger<HangfireJobs> _logger;

    public HangfireJobs(
        IEarthquakeDataService earthquakeDataService,
        IKafkaProducerService kafkaProducerService,
        //IEmailService emailService, // Commented out for now
        ILogger<HangfireJobs> logger)
    {
        _earthquakeDataService = earthquakeDataService;
        _kafkaProducerService = kafkaProducerService;
        //_emailService = emailService; // Commented out for now
        _logger = logger;
    }

    public async Task FetchAndLogLatestEarthquakeData()
    {
        try
        {
            var latestEarthquake = await _earthquakeDataService.GetLatestEarthquakeDataAsync();
            _logger.LogInformation($"Latest Earthquake Event: {latestEarthquake?.Location}");

            if (latestEarthquake != null)
            {
                // Produce message to Kafka
                await _kafkaProducerService.ProduceMessageAsync(Guid.NewGuid().ToString(), latestEarthquake);

                // Prepare email details (commented out)
                //var emailSubject = $"Latest Earthquake Event: {latestEarthquake.Location}";
                //var emailBody = $"A new earthquake has been detected!\n\n" +
                //                $"Location: {latestEarthquake.Location}\n" +
                //                $"Magnitude: {latestEarthquake.Magnitude}\n" +
                //                $"Date: {latestEarthquake.Date}\n";

                // Send email notification (commented out)
                //await _emailService.SendEmailAsync("ipek.oktay52@gmail.com", emailSubject, emailBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the latest earthquake data.");
        }
    }

    public async Task FetchAndLogDailyEarthquakeData(DateTime date)
    {
        try
        {
            var startDate = date.Date.ToUniversalTime();
            var endDate = startDate.AddDays(1).AddTicks(-1);

            var earthquakes = await _earthquakeDataService.GetEarthquakesByDateAsync(startDate, endDate);
            _logger.LogInformation($"Earthquakes for {startDate:yyyy-MM-dd}: {earthquakes.Count}");

            foreach (var earthquake in earthquakes)
            {
                await _kafkaProducerService.ProduceMessageAsync(Guid.NewGuid().ToString(), earthquake);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching daily earthquake data.");
        }
    }

    public async Task FetchAndLogDailyEarthquakeDataWithMagnitude(DateTime date, double magnitude)
    {
        try
        {
            var startDate = date.Date.ToUniversalTime();
            var endDate = startDate.AddDays(1).AddTicks(-1);

            var earthquakes = await _earthquakeDataService.GetEarthquakesByDateAsync(startDate, endDate);

            var filteredEarthquakes = earthquakes
                .Where(e => double.TryParse(e.Magnitude, out var parsedMagnitude) && parsedMagnitude >= magnitude)
                .ToList();

            _logger.LogInformation($"Earthquakes for {startDate:yyyy-MM-dd} with magnitude >= {magnitude}: {filteredEarthquakes.Count}");

            foreach (var earthquake in filteredEarthquakes)
            {
                await _kafkaProducerService.ProduceMessageAsync(Guid.NewGuid().ToString(), earthquake);
            }

            if (filteredEarthquakes.Any())
            {
                var emailSubject = $"Earthquakes with Magnitude >= {magnitude} Detected on {date:yyyy-MM-dd}";
                var emailBody = string.Join(Environment.NewLine, filteredEarthquakes.Select(e => $"Location: {e.Location}, Magnitude: {e.Magnitude}, Date: {e.Date}"));

                // Send email notification (commented out)
                //await _emailService.SendEmailAsync("ipek.oktay52@gmail.com", emailSubject, emailBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching daily earthquake data with magnitude threshold.");
        }
    }
}
