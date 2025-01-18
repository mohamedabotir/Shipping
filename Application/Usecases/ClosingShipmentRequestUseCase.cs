using Common.Events;
using Common.Repository;
using Common.Result;
using Domain.Repositories;

namespace Application.Usecases;

public class ClosingShipmentRequestUseCase(IShippingRepository shippingRepository,IUnitOfWork unitOfWork):IClosingShipmentRequestUseCase
{
    public async Task<Result> CloseShipment(OrderClosed @event)
    {
            var result = await shippingRepository.GetShippingOrderByPurchaseOrderNumberWithFactory(@event.PoNumber);
            if(result.IsFailure)
                return Result.Fail(result.Message);
            var shipment  = result.Value.MarkOrderAsDelivered();
            if(shipment.IsFailure)
                return Result.Fail(shipment.Message);
            await shippingRepository.UpdateShippingStageWithFactory((int)result.Value.Id,result.Value.PackageOrder.OrderStage);
            return Result.Ok();
    }
}