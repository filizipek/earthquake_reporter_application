using Confluent.Kafka;
using KafkaFlow;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using EarthquakeReporter.Models;
using KafkaFlow.Producers;

public interface IKafkaProducerService
{
    Task ProduceMessageAsync(string key, EarthquakeEvent earthquakeEvent);
}

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<IKafkaProducerService> _logger;

    public KafkaProducerService(IServiceProvider provider, ILogger<IKafkaProducerService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task ProduceMessageAsync(string key, EarthquakeEvent earthquakeEvent)
    {
        try
        {
            var accessor = _provider.GetRequiredService<IProducerAccessor>();
            var producer = accessor.GetProducer("earthquake-events");

            await producer.ProduceAsync(key, earthquakeEvent);
            _logger.LogInformation("Message produced successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing message.");
        }
    }
}
