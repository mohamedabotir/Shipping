using System.Text.Json;
using Common.Events;
using Common.Mongo.Producers;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Infrastructure.MessageBroker.Producers;

public class Producer:IProducer
{
    public ProducerConfig _config { get; set; }
    public Producer(IOptions<ProducerConfig> options)
    {
        _config = options.Value;
    }
    public async Task ProduceAsync<T>(string topic, T @event) where T : DomainEventBase
    {
        try
        {
            using var producer = new ProducerBuilder<string, string>(_config)
                .SetValueSerializer(Serializers.Utf8)
                .SetKeySerializer(Serializers.Utf8)
                .Build();
            Message<string, string> deliverMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event, @event.GetType())
            };
            var result = await producer.ProduceAsync(topic, deliverMessage);
            if (result.Status == PersistenceStatus.NotPersisted)
                throw new Exception($"Cannot produce {@event.GetType()} to topic {topic} due to {result.Message}");
        }
        catch (Exception ex)
        {

            throw;
        }
            
    }
}
