using Common.Events;
using Common.Mongo.Producers;
using Common.Repository;
using Common.Result;
using Confluent.Kafka;
using Domain.Entity;
using Infrastructure.MessageBroker;
using Microsoft.Extensions.Options;
using EventModel = Common.Events.EventModel;

namespace Infrastructure.Mongo;

public class PurchaseOrderEventStore(IEventRepository eventRepository, IProducer producer, IOptions<TopicShippingOrders> options) : IEventStore
{

    public async Task SaveEventAsync(Guid aggregateId, DomainEventBase eventBase,List<Maybe<string>> anotherTopics)
    {
 
            var eventModel = new EventModel
            {
                TimeStamp = DateTime.Now,
                AggregateType = nameof(ShippingOrder),
                AggregateIdentifier = aggregateId,
                EventBaseData = eventBase,
                EventType = eventBase.GetType().Name
            };
            await eventRepository.SaveEventAsync(eventModel);

            var topicName = options.Value.TopicName;
            await producer.ProduceAsync(topicName, eventBase);
            var shipmentTopics = anotherTopics.Where(e=>e.HasValue)
                .Select(e => e.Value).ToList();
            foreach (var topic in shipmentTopics)
            {
                await producer.ProduceAsync(topic, eventBase);
            }
 
    }

    public Task<List<DomainEventBase>> GetEventsAsync(Guid aggregateId, string collectionName = "")
    {
        throw new NotImplementedException();
    }

}