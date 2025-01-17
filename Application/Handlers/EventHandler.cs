using Common.Events;
using Domain.Repositories;
using Infrastructure.Consumer;
using Infrastructure.Consumer.Usecases;

namespace Application.Handlers;

public class EventHandler(IPlaceShipmentRequestUsecase shipmentRequestUsecase):IEventHandler
{

    public  async Task On(PurchaseOrderApproved @event)
    {
      await  shipmentRequestUsecase.CreateShipment(@event);
    }
}