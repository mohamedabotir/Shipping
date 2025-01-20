using Common.Events;

namespace Common.Handlers;

public interface IEventDispatcher
{
    Task DispatchDomainEventsAsync(IEnumerable<DomainEventBase> domainEvents);
}