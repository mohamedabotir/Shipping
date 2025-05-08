using Common.Events;
using Common.Repository;
using Common.Result;
using Common.ValueObject;
using Domain.Entity;
using Domain.Repositories;
using Domain.ValueObject;

namespace Infrastructure.Consumer.Usecases;

public class PlaceShipmentRequestUsecase(IShippingRepository shippingRepository,IUnitOfWork<ShippingOrder> unitOfWork):IPlaceShipmentRequestUsecase
{
    public async Task<Result> CreateShipment(PurchaseOrderApproved request)
    {
       
            var address = Address.CreateInstance(request.CustomerAddress);
            if (address.IsFailure)
                return Result.Fail(address.Message);
            var user = User.CreateInstance(request.CustomerName, address.Value,request.CustomerPhoneNumber);
            if (user.IsFailure)
                return Result.Fail(user.Message);
            var package = new PackageOrder(request.TotalAmount,request.ActivationStatus,request.PurchaseOrderNumber,request.OrderStage,
                request.PurchaseOrderId);
            var shippingOrder =  ShippingOrder.CreateShippingOrder(Guid.NewGuid(),0, user.Value,package, request);
            await shippingRepository.AddAsync(shippingOrder);
            await unitOfWork.SaveChangesAsync(shippingOrder);
            return Result.Ok();
        
    }
}