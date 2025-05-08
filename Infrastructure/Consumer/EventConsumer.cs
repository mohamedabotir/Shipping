using System.Text.Json;
using Application.Handlers;
using Common.Events;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Consumer;

public sealed class EventConsumer(
    IOptions<ConsumerConfig> consumerConfig,
    IServiceScopeFactory serviceScopeFactory)
    : IEventConsumer<EventConsumer>
{
    private readonly ConsumerConfig _consumerConfig = consumerConfig.Value;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public void Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(_consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);

        while (true)
        {
            ConsumeResult<string, string>? consumeResult = null;

            try
            {
                consumeResult = consumer.Consume();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kafka consumption failed: {ex.Message}");
                continue;
            }

            if (consumeResult?.Message == null) continue;

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var eventHandler = scope.ServiceProvider.GetRequiredService<IEventHandler>();

                var options = new JsonSerializerOptions
                {
                    Converters = { new EventJsonConverter() }
                };

                var @event = JsonSerializer.Deserialize<DomainEventBase>(consumeResult.Message.Value, options);

                if (@event == null)
                {
                    Console.WriteLine("Failed to deserialize event");
                    continue;
                }

                var handlerMethod = eventHandler.GetType()
                    .GetMethod("On", new[] { @event.GetType() });

                if (handlerMethod == null)
                {
                    Console.WriteLine($"No handler found for event type: {@event.GetType().Name}");
                    continue;
                }

                var task = (Task?)handlerMethod.Invoke(eventHandler, new object[] { @event });

                if (task != null)
                    task.GetAwaiter().GetResult();

                consumer.Commit(consumeResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while processing message: {ex.Message}");
            }
        }
    }
}
