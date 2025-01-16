using Common.Result;
using Domain.Entity;
using Domain.Repositories;

namespace Infrastructure.Repository;

public class ShippingRepository: IShippingRepository
{
    public Task<Result> Save(ShippingOrder shippingOrder)
    {
       Console.Write($"Document being processed : {shippingOrder.PackageOrder.PurchaseOrderNumber}");
       throw new NotImplementedException();
    }
}