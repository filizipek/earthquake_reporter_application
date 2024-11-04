using EarthquakeReporter.Models;
using EarthquakeReporter.Services;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;

namespace EarthquakeReporter.Configuration
{
    public static class KafkaConfiguration
    {
        public static void ConfigureKafkaFlow(this IServiceCollection services)
        {
            services.AddKafka(kafka => kafka
                .AddCluster(cluster => cluster
                    .CreateTopicIfNotExists("earthquake-events", 1, 1)
                    .WithBrokers(new[] { "localhost:9092" }) // Update with your Kafka broker address
                    .AddProducer("earthquake-events", producer => producer
                        .DefaultTopic("earthquake-events") // Ensure the topic exists or enable auto-creation
                        //.WithAcks(Acks.All) // Ensures all in-sync replicas acknowledge
                        .AddMiddlewares(x => x.AddSerializer<JsonCoreSerializer>())
                    )
                    .AddConsumer(consumer => consumer
                        .WithBufferSize(100)
                        .WithWorkersCount(1)
                        .Topic("earthquake-events").WithGroupId("earthquake-consumer-group")
                        .AddMiddlewares(middlewares => middlewares
                            .AddDeserializer<JsonCoreDeserializer, CustomMessageTypeResolver>()
                            .AddTypedHandlers(handlers => handlers
                                .AddHandler<EarthquakeMessageHandler>()
                                .WithHandlerLifetime(InstanceLifetime.Scoped))))
                )
            );
        }
    }
}