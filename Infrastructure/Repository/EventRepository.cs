using Common.Repository;
using Infrastructure.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using EventModel = Common.Events.EventModel;

namespace Infrastructure.Repository;

public class EventRepository:IEventRepository
{
        private  IMongoCollection<EventModel> _eventCollection;
        private readonly IMongoDatabase _mongoClient;
        private readonly string _collectionName;
        public EventRepository(IOptions<ShippingOrderConfig> options)
        {
            MongoClient client = new MongoClient(options.Value.ConnectionString);
            _mongoClient= client.GetDatabase(options.Value.DatabaseName);
            _collectionName = options.Value.CollectionName;
            _eventCollection = _mongoClient.GetCollection<EventModel>(_collectionName);
        }
        public async Task<List<EventModel>> GetAggregate(Guid aggregateId)
        {
            try
            {
                _eventCollection = _mongoClient.GetCollection<EventModel>(_collectionName);

                return await _eventCollection.Find(e => e.AggregateIdentifier == aggregateId).ToListAsync();
            }
            catch
            {

                throw;
            }

 
        }

        public Task<List<EventModel>> GetAggregateByUserName(string name)
        {
            try
            {
                //var loggedData = await _eventCollection.Find(e => e.EventData.GetType() == typeof(UserLoggedInEvent) && (e.EventData as UserLoggedInEvent).Name == name).ToListAsync();
                //return loggedData.Any()? loggedData: await _eventCollection.Find(e => e.EventData.GetType() == typeof(UserCreatedEvent) && (e.EventData as UserCreatedEvent).Name == name).ToListAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
            return (Task<List<EventModel>>)Task.CompletedTask;
        }

        public async Task SaveEventAsync(EventModel @event)
        {
          
            _eventCollection = _mongoClient.GetCollection<EventModel>(_collectionName);

            await _eventCollection.InsertOneAsync(@event).ConfigureAwait(true);

        }
    }
