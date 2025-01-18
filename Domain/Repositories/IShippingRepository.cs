using Common.Constants;
using Common.Result;
using Domain.Entity;

namespace Domain.Repositories;

public interface IShippingRepository
{
    Task Save(ShippingOrder shippingOrder);
    Task<Result<ShippingOrder>> GetShippingOrderByPurchaseOrderNumber(string purchaseOrderNumber);
    Task UpdateShippingStage(int orderId, PurchaseOrderStage stage);

}