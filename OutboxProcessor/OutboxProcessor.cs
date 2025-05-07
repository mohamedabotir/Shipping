using Common.Events;
using Common.Mongo.Producers;
using Common.Repository;
using Infrastructure.Consumer;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Serilog;

public class OutboxProcessor
{
    private readonly IEventRepository _eventRepo;
    private readonly IProducer _producer;
    private readonly string _topic;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly EventTopicMapping _topicMapping;


    public OutboxProcessor(IEventRepository eventRepo, IProducer producer, IOptions<Topic> options, IOptions<EventTopicMapping> eventTopicMapping)
    {
        _eventRepo = eventRepo;
        _producer = producer;
        _topic = options.Value.TopicName;
        _topicMapping = eventTopicMapping.Value;

        _retryPolicy = Policy
    .Handle<Exception>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        onRetry: (exception, timeSpan, retryCount, context) =>
        {
            Log.Information($"🔁 Retry {retryCount} after {timeSpan.TotalSeconds}s due to: {exception.Message}");
        });
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {

        var unprocessed = await _eventRepo.GetUnprocessedEventsAsync();
        foreach (var evt in unprocessed)
        {
            try
            {
                    var topics = GetTopicsForEvent(evt.EventBaseData);
                    foreach (var topic in topics)
                    {
                        await _producer.ProduceAsync(topic, evt.EventBaseData);
                    }
                await _eventRepo.MarkAsProcessed(evt.Id); 
            }
            catch (Exception ex)
            {
                Log.Error($"❌ Failed to process event {evt.Id}: {ex.Message}");
            }
        }
        }
        catch (Exception ex)
        {
            Log.Error($"❌ Failed to process : {ex.Message}");

        }

        Log.Error("✅ Finished processing outbox events.");
    }

    private List<string> GetTopicsForEvent(DomainEventBase domainEvent)
    {
        var eventType = domainEvent.GetType().Name;
        return _topicMapping.TopicMappings.TryGetValue(eventType, out var topics)
            ? topics
            : new List<string> { _topic };
    }
}
