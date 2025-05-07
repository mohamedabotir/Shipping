using Application.Commands;
using Common.Repository;
using Common.Result;
using Domain.Entity;
using Domain.Repositories;

namespace Application.Usecases;

public class OrderShippedUseCase(IShippingRepository shippingRepository,IUnitOfWork<ShippingOrder> unitOfWork):IOrderShippedUseCase
{
    public async Task<Result> MarkDocumentAsShipped(OrderShippedCommand shippedCommand)
    {
        using (unitOfWork)
        {
            var shippingOrder =await shippingRepository
                .GetShippingOrderByPurchaseOrderNumber(shippedCommand.purchaseNumber);
            if(shippingOrder.IsFailure)
                return Result.Fail(shippingOrder.Message);
            var shipmentResult = shippingOrder.Value.MarkOrderAsShipped();
            if (shipmentResult.IsFailure)
                return Result.Fail(shippingOrder.Message);
            await shippingRepository.UpdateShippingStage((int)shippingOrder.Value.Id,
                shippingOrder.Value.PackageOrder.OrderStage);
            await unitOfWork.SaveChangesAsync(shippingOrder.Value);
            return Result.Ok();
        }    
    }
}