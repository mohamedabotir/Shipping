using Common.Events;
using Common.Handlers;

namespace Infrastructure.EventHandlers;

public class OrderShippedHandler(IEventStore eventStore):IEventHandler<OrderShipped>
{
    public async Task HandleAsync(OrderShipped @event, CancellationToken cancellationToken = default)
    {
        await  eventStore.SaveEventAsync(@event.PurchaseOrderGuid,@event,["purchaseOrder","inventoryTrack"]);
    }
}