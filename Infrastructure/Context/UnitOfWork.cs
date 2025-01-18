using Common.Entity;
using Common.Events;
using Common.Handlers;
using Common.Repository;
using Domain.Repositories;
using Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Consumer.Context;

public class UnitOfWork(ShippingOrderContext dbContext, IServiceProvider serviceProvider,IEventDispatcher eventDispatcher)
    : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(IEnumerable<DomainEventBase> events,CancellationToken cancellationToken = default)
    {
     
        var result = await dbContext.SaveChangesAsync(cancellationToken);
        if (events.Any())
            await eventDispatcher.DispatchDomainEventsAsync(events);
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