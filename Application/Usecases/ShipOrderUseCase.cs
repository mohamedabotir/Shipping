using Application.Commands;
using Common.Events;
using Common.Repository;
using Common.Result;
using Domain.Entity;
using Domain.Repositories;
using Infrastructure.Consumer.Usecases;

namespace Application.Usecases;

public class ShipOrderUseCase(IEventSourcing<ShippingOrder> eventSourcing,IShippingRepository shippingRepository,IUnitOfWork<ShippingOrder> unitOfWork):IShipOrderUsecase
{
    public async Task<Result> ShipOrder(StartShippingCommand command)
    {
        using (unitOfWork)
        {
            var shippingOrder = await eventSourcing.GetByIdAsync(command.purchaseNumber);
            
            var shipmentResult = shippingOrder.ShipOrder();
            
            await shippingRepository.UpdateShippingStageByPurchaseNumber(shippingOrder.PackageOrder.PurchaseOrderNumber,
                shippingOrder.PackageOrder.OrderStage);
            await unitOfWork.SaveChangesAsync(shippingOrder);
            return Result.Ok();
        }
    }
}