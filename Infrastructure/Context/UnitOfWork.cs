using Common.Entity;
using Common.Events;
using Common.Repository;
using Domain.Repositories;
using Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Consumer.Context;

public class UnitOfWork(ShippingOrderContext dbContext, IServiceProvider serviceProvider)
    : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(IEnumerable<DomainEventBase> events,CancellationToken cancellationToken = default)
    {
     
        var result = await dbContext.SaveChangesAsync(cancellationToken);

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