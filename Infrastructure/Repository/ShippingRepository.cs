using Domain.Entity;
using Domain.Repositories;
using Infrastructure.Consumer.Context;
using Infrastructure.Consumer.Context.Pocos;
using Infrastructure.Context;

namespace Infrastructure.Consumer.Repository;

public class ShippingRepository: IShippingRepository
{
    private readonly ShoppingContextFactory _shippingOrderContext;
    public ShippingRepository(ShoppingContextFactory shippingOrderContext)
    {
        _shippingOrderContext = shippingOrderContext;
    }
    public async Task Save(ShippingOrder shippingOrder)
    {
        await using var context = _shippingOrderContext.CreateDataBaseContext();
        var entity = new ShippingOrderPoco()
            .MapDomainToPoco(shippingOrder);
       await context
           .AddAsync(entity);
       var result = await context.SaveChangesAsync();
    }
}