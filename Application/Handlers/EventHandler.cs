using Application.Usecases;
using Common.Events;
using Common.Result;
 
namespace Infrastructure.Consumer;

public class EventHandler(IPlaceShipmentRequestUsecase shipmentRequestUsecase):IEventHandler
{
   
    public async Task<Result> On(PurchaseOrderApproved @event)
    {
        return await shipmentRequestUsecase.CreateShipment(@event);
    }
}