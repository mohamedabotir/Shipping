using Common.Entity;
using Common.Events;
using Common.Handlers;
using Common.Repository;
using Domain.Entity;
using Domain.Repositories;
using Infrastructure.Context;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Consumer.Context;

public class UnitOfWork(ShippingOrderContext dbContext, IServiceProvider serviceProvider,IEventDispatcher eventDispatcher,
    IHttpContextAccessor httpContextAccessor, IEventSourcing<ShippingOrder> eventSourcing)
    : IUnitOfWork<ShippingOrder>
{
    public async Task<int> SaveChangesAsync(ShippingOrder aggregate,CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var correlationId = string.Empty;

        try
        {
            correlationId = httpContextAccessor.HttpContext!.GetCorrelationId();
            var result = await dbContext.SaveChangesAsync(cancellationToken);

            await eventSourcing.SaveAsync(aggregate);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            var exceptionMessage =
                $"An error occured while Process Request please use this correlation id and contact help desk team {correlationId}";
            throw new InvalidOperationException(exceptionMessage, ex);
        }
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