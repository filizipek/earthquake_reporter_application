using System;
using System.Linq;
using System.Threading.Tasks;
using EarthquakeReporter.Models;
using EarthquakeReporter.Repositories;
using KafkaFlow;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EarthquakeReporter.Services
{
    public class EarthquakeMessageHandler : IMessageHandler<EarthquakeEvent>
    {
        private readonly IEarthquakeEventRepository _earthquakeEventRepository;
        private readonly ILogger<EarthquakeMessageHandler> _logger;

        public EarthquakeMessageHandler(IEarthquakeEventRepository earthquakeEventRepository, ILogger<EarthquakeMessageHandler> logger)
        {
            _earthquakeEventRepository = earthquakeEventRepository;
            _logger = logger;
        }

        public async Task Handle(IMessageContext context, EarthquakeEvent message)
        {
            try
            {
                _logger.LogInformation($"Received earthquake event: {message.Location}");

                var existingEvents = await _earthquakeEventRepository.GetByDateAsync(message.Date);
                
                if (!existingEvents.Any())
                {
                    await _earthquakeEventRepository.CreateAsync(message);
                    _logger.LogInformation($"Created new earthquake event: {message.District}");
                }
                else
                {
                    await _earthquakeEventRepository.UpdateAsync(message);
                    _logger.LogInformation($"Updated existing earthquake event: {message.District}");
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError($"SQL Error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing earthquake event: {ex.Message}");
            }
        }
    }
}
