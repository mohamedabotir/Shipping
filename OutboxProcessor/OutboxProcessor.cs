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
    private readonly string _defaultTopic;
    private readonly EventTopicMapping _topicMapping;
    private readonly AsyncRetryPolicy _retryPolicy;

    public OutboxProcessor(
        IEventRepository eventRepo,
        IProducer producer,
        IOptions<Topic> options,
        IOptions<EventTopicMapping> eventTopicMapping)
    {
        _eventRepo = eventRepo;
        _producer = producer;
        _defaultTopic = options.Value.TopicName;
        _topicMapping = eventTopicMapping.Value;

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Log.Warning($"Retry {retryCount} for event {context["EventId"]} after {timeSpan.TotalSeconds}s: {exception.Message}");
                });
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            var unprocessed = await _eventRepo.GetUnprocessedEventsAsync();

            foreach (var evt in unprocessed)
            {
                var context = new Context();
                context["EventId"] = evt.Id.ToString();

                try
                {
                    await _retryPolicy.ExecuteAsync(async ctx =>
                    {
                        var topics = GetTopicsForEvent(evt.EventBaseData);

                        foreach (var topic in topics)
                        {
                            await _producer.ProduceAsync(topic, evt.EventBaseData);
                        }

                        await _eventRepo.MarkAsProcessed(evt.Id);

                    }, context);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed after 3 retries for event {evt.Id}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Fatal($"Critical failure in outbox processor: {ex.Message}");
        }

        Log.Information("Finished processing outbox events.");
    }

    private List<string> GetTopicsForEvent(DomainEventBase domainEvent)
    {
        var eventType = domainEvent.GetType().Name;
        return _topicMapping.TopicMappings.TryGetValue(eventType, out var topics)
            ? topics
            : new List<string> { _defaultTopic };
    }
}

