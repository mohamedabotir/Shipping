using Common.Events;
using Common.Entity;

namespace Common.Repository;

public interface IUnitOfWork: IDisposable
{
    public  Task<int> SaveChangesAsync(IEnumerable<DomainEventBase> events,CancellationToken cancellationToken = default);
    public IRepository<T>? GetRepository<T>() where T : AggregateRoot;
}