using Common.Events;
using Common.Handlers;

namespace Infrastructure.EventHandlers;

public class OrderBeingShippedHandler(IEventStore eventStore):IEventHandler<OrderBeingShipped>
{
    public async Task HandleAsync(OrderBeingShipped @event, CancellationToken cancellationToken = default)
    {
        await  eventStore.SaveEventAsync(@event.PurchaseOrderGuid,@event,["purchaseOrder"]);
    }
}