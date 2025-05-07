using Application.Commands;
using Common.Events;
using Common.Repository;
using Common.Result;
using Domain.Entity;
using Domain.Repositories;

namespace Application.Usecases;

public class OrderShippedUseCase(IEventSourcing<ShippingOrder> eventSourcing,IShippingRepository shippingRepository,IUnitOfWork<ShippingOrder> unitOfWork):IOrderShippedUseCase
{
    public async Task<Result> MarkDocumentAsShipped(OrderShippedCommand shippedCommand)
    {
        using (unitOfWork)
        {
            var shippingOrder = await eventSourcing.GetByIdAsync(shippedCommand.purchaseNumber);
            var shipmentResult = shippingOrder.MarkOrderAsShipped();
            if (shipmentResult.IsFailure)
                return Result.Fail(shipmentResult.Message);
            await shippingRepository.UpdateShippingStage((int)shippingOrder.Id,
                shippingOrder.PackageOrder.OrderStage);
            await unitOfWork.SaveChangesAsync(shippingOrder);
            return Result.Ok();
        }    
    }
}