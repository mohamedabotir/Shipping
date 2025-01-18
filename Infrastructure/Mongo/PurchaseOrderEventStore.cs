using Common.Events;
using Common.Mongo.Producers;
using Common.Repository;
using Confluent.Kafka;
using Domain.Entity;
using Infrastructure.MessageBroker;
using Microsoft.Extensions.Options;
using EventModel = Common.Events.EventModel;

namespace Infrastructure.Mongo;

public class PurchaseOrderEventStore(IEventRepository eventRepository, IProducer producer, IOptions<TopicShippingOrders> options) : IEventStore
{

    public async Task SaveEventAsync(Guid aggregateId, DomainEventBase eventBase)
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
 
    }

    public Task<List<DomainEventBase>> GetEventsAsync(Guid aggregateId, string collectionName = "")
    {
        throw new NotImplementedException();
    }

}