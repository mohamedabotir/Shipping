using Common.Events;

namespace Common.Entity;

public class AggregateRoot : Entity
{
    public virtual Guid Guid { get; protected internal set; }
    private readonly List<DomainEventBase> _domainEvents = new List<DomainEventBase>();
    public virtual IReadOnlyList<DomainEventBase> DomainEvents => _domainEvents;

    protected virtual void AddDomainEvent(DomainEventBase newEventBase)
    {
        _domainEvents.Add(newEventBase);
    }

    public virtual void ClearEvents()
    {
        _domainEvents.Clear();
    }
}