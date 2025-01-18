using Common.Events;
using Common.Handlers;

namespace Application.Handlers;

public class OrderBeingShippedHandler(IEventStore eventStore):IEventHandler<OrderBeingShipped>
{
    public async Task HandleAsync(OrderBeingShipped @event, CancellationToken cancellationToken = default)
    {
        await  eventStore.SaveEventAsync(@event.PurchaseOrderGuid,@event);
    }
}