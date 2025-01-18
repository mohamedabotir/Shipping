using Application.Usecases;
using Common.Events;
using Domain.Repositories;
using Infrastructure.Consumer;
using Infrastructure.Consumer.Usecases;

namespace Application.Handlers;

public class EventHandler(IPlaceShipmentRequestUsecase shipmentRequestUsecase,IClosingShipmentRequestUseCase closingShipmentRequestUseCase):IEventHandler
{

    public  async Task On(PurchaseOrderApproved @event)
    {
      await  shipmentRequestUsecase.CreateShipment(@event);
    }

    public async Task On(OrderClosed @event)
    {
        await closingShipmentRequestUseCase.CloseShipment(@event);
    }
}