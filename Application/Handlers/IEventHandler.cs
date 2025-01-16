using Common.Events;
using Common.Result;

namespace Infrastructure.Consumer;

public interface IEventHandler
{
    public  Task<Result> On(PurchaseOrderApproved @event);
}