using Common.Entity;
using Common.Events;
using Common.Handlers;
using Common.Repository;
using Domain.Entity;
using Domain.Repositories;
using Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Consumer.Context;

public class UnitOfWork(ShippingOrderContext dbContext, IServiceProvider serviceProvider,IEventDispatcher eventDispatcher, IEventSourcing<ShippingOrder> eventSourcing)
    : IUnitOfWork<ShippingOrder>
{
    public async Task<int> SaveChangesAsync(ShippingOrder aggregate,CancellationToken cancellationToken = default)
    {
     
        var result = await dbContext.SaveChangesAsync(cancellationToken);
        if (aggregate.GetUncommittedEvents().Any())
        await eventSourcing.SaveAsync(aggregate);

        return result;
    }


    public IRepository<T>? GetRepository<T>() where T : AggregateRoot
    {
        return serviceProvider.GetService<IRepository<T>>();
    }
    public void Dispose()
    {
        dbContext?.Dispose();
        GC.SuppressFinalize(this);
    }
}