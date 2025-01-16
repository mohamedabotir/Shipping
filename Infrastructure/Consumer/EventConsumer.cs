using System.Text.Json;
using Common.Events;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Consumer;

     public sealed class EventConsumer(
         IOptions<ConsumerConfig> consumerConfig,
         IEventHandler eventHandler,
         IServiceScopeFactory serviceProvider)
         : IEventConsumer<EventConsumer>
     {
        private ConsumerConfig _consumerConfig { get; } = consumerConfig.Value;
        private IEventHandler _eventHandler { get;  } = eventHandler;
        private IServiceScopeFactory _serviceProvider { get; } = serviceProvider;

        public void Consume(string Topic)
        {
            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig)
                            .SetKeyDeserializer(Deserializers.Utf8)
                            .SetValueDeserializer(Deserializers.Utf8)
                            .Build();
            consumer.Subscribe(Topic);
            while (true)
            {
                ConsumeResult<string, string> consumeResult = null;

                try
                {

                    consumeResult = consumer.Consume();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed :{0}", ex.Message);
                    throw;
                }
                if (consumeResult?.Message == null) continue;
                try
                {

               
                using (var scope = _serviceProvider.CreateScope())
                {
                    
                    using (var sc = scope.ServiceProvider.CreateScope())
                    {

                        var options = new JsonSerializerOptions() { Converters = { new EventJsonConverter() } };
                        var @event = JsonSerializer.Deserialize<DomainEventBase>(consumeResult.Message.Value, options);
                        var handlers = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });

                        if (handlers == null)
                            throw new ArgumentNullException($"Handler Didn't support method with type : {@event.GetType().Name} ");
                        handlers.Invoke(_eventHandler, new Object[] { @event });
                        consumer.Commit(consumeResult);

                    }
                }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
