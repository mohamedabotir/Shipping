using Common.Entity;
using Common.Events;
using Common.Repository;
using Domain.Entity;


namespace Infrastructure.Mongo
{
    public class EventSourcing : IEventSourcing<ShippingOrder>
    {
        public IEventRepository _eventStore { get; set; }
        public EventSourcing(IEventRepository eventStore)
        {
            _eventStore = eventStore;
        }
        public async Task<ShippingOrder> GetByIdAsync(string id, string collectionName = "")
        {
            var aggregate = new ShippingOrder();
            var @event = (await _eventStore.GetAggregate(id)).Select(e => e.EventBaseData);

            aggregate.ReplayEvents(@event);

            aggregate.Version = @event.Select(e => e.Version).Max();
            return aggregate;
        }

        public async Task SaveAsync(ShippingOrder entity, string topicName = "", string collectionName = "")
        {

            await _eventStore.SaveEventAsync(entity.PackageOrder.PurchaseOrderNumber, entity.GetUncommittedEvents(), entity.Version, false, topicName, collectionName);
            entity.MarkChangesAsCommitted();
        }
    }
}
