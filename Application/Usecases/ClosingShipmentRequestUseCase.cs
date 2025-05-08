using Common.Events;
using Common.Repository;
using Common.Result;
using Domain.Entity;
using Domain.Repositories;

namespace Application.Usecases;

public class ClosingShipmentRequestUseCase(IEventSourcing<ShippingOrder> eventSourcing, IShippingRepository shippingRepository,IUnitOfWork<ShippingOrder> unitOfWork):IClosingShipmentRequestUseCase
{
    public async Task<Result> CloseShipment(OrderClosed @event)
    {
        try
        {

       
            var result = await eventSourcing.GetByIdAsync(@event.PoNumber);
           
            var shipment  = result.MarkOrderAsDelivered(@event);
            if(shipment.IsFailure)
                return Result.Fail(shipment.Message);
            await shippingRepository.UpdateShippingStageByPurchaseNumber(result.PackageOrder.PurchaseOrderNumber,result.PackageOrder.OrderStage);
        await unitOfWork.SaveChangesAsync(result);
            return Result.Ok();
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }
}