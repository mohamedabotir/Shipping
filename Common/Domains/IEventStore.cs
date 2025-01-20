using Common.Result;

namespace Common.Events;

public interface IEventStore
{
    Task SaveEventAsync(Guid aggregateId, DomainEventBase eventBase,List<Maybe<string>> anotherTopics = (List<Maybe<string>>)default);
    
    Task<List<DomainEventBase>> GetEventsAsync(Guid aggregateId,string collectionName="");
}