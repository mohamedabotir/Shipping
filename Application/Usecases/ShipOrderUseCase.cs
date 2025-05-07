using Application.Commands;
using Common.Repository;
using Common.Result;
using Domain.Entity;
using Domain.Repositories;
using Infrastructure.Consumer.Usecases;

namespace Application.Usecases;

public class ShipOrderUseCase(IShippingRepository shippingRepository,IUnitOfWork<ShippingOrder> unitOfWork):IShipOrderUsecase
{
    public async Task<Result> ShipOrder(StartShippingCommand command)
    {
        using (unitOfWork)
        {
            var shippingOrder =await shippingRepository
                .GetShippingOrderByPurchaseOrderNumber(command.purchaseNumber);
            if(shippingOrder.IsFailure)
                return Result.Fail(shippingOrder.Message);
            var shipmentResult = shippingOrder.Value.ShipOrder();
            if (shipmentResult.IsFailure)
                return Result.Fail(shippingOrder.Message);
            await shippingRepository.UpdateShippingStage((int)shippingOrder.Value.Id,
                shippingOrder.Value.PackageOrder.OrderStage);
            await unitOfWork.SaveChangesAsync(shippingOrder.Value);
            return Result.Ok();
        }
    }
}