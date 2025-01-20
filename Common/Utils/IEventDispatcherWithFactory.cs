using Common.Events;
using Common.Handlers;

namespace Common.Utils;

public interface IEventDispatcherWithFactory
{
      Task DispatchDomainEventAsync(IEnumerable<DomainEventBase> domainEvents);

}