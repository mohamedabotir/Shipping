using Common.Events;

namespace Application.Handlers;

public interface IEventHandler
{
    public  Task On(PurchaseOrderApproved @event);
    public  Task On(OrderClosed @event);

}