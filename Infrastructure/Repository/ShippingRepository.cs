using Common.Constants;
using Common.Result;
using Domain.Entity;
using Domain.Repositories;
using Infrastructure.Consumer.Context;
using Infrastructure.Consumer.Context.Pocos;
using Infrastructure.Context;

namespace Infrastructure.Consumer.Repository;

public class ShippingRepository: IShippingRepository
{
    private readonly ShippingContextFactory _shoppingContextFactory;
    private ShippingOrderContext _shippingOrderContext;

    public ShippingRepository(ShippingContextFactory shippingOrderContext,ShippingOrderContext shoppingContext)
    {
        _shoppingContextFactory = shippingOrderContext;
        _shippingOrderContext = shoppingContext;
    }

    public async Task Save(ShippingOrder shippingOrder)
    {
        await using var context = _shoppingContextFactory.CreateDataBaseContext();
        var entity = new ShippingOrderPoco()
            .MapDomainToPoco(shippingOrder);
       await context
           .AddAsync(entity);
       var result = await context.SaveChangesAsync();
    }

    public  Task<Result<ShippingOrder>> GetShippingOrderByPurchaseOrderNumber(string purchaseOrderNumber)
    {
        var result =_shippingOrderContext.ShippingOrder
                .SingleOrDefault(e => e.PurchaseOrderNumber == purchaseOrderNumber)
                ?.MapPocoToDomain();
        return Task.FromResult(result)!;
    }

    public Task UpdateShippingStage(int orderId,PurchaseOrderStage stage)
    {
        var shippingOrder = _shippingOrderContext.ShippingOrder
            .Single(e => e.OrderId == orderId);
        shippingOrder.OrderStage = stage;
      return  Task.CompletedTask;
    }

    public Task<Result<ShippingOrder>> GetShippingOrderByPurchaseOrderNumberWithFactory(string purchaseOrderNumber)
    {
        using var context = _shoppingContextFactory.CreateDataBaseContext();
        _shippingOrderContext = context;
        return GetShippingOrderByPurchaseOrderNumber(purchaseOrderNumber);
    }

    public Task UpdateShippingStageWithFactory(int orderId, PurchaseOrderStage stage)
    {
        using var context = _shoppingContextFactory.CreateDataBaseContext();
        _shippingOrderContext = context;
        UpdateShippingStage(orderId, stage);
        context.SaveChanges();
        return Task.CompletedTask;
    }
}