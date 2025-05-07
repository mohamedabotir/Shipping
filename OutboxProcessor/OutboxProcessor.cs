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


    public OutboxProcessor(IEventRepository eventRepo, IProducer producer, IOptions<Topic> options)
    {
        _eventRepo = eventRepo;
        _producer = producer;
        _topic = options.Value.TopicName;
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
        var unprocessed = await _eventRepo.GetUnprocessedEventsAsync();
        foreach (var evt in unprocessed)
        {
            try
            {
                await _producer.ProduceAsync(_topic, evt.EventBaseData);
                await _eventRepo.MarkAsProcessed(evt.Id); 
            }
            catch (Exception ex)
            {
                Log.Error($"❌ Failed to process event {evt.Id}: {ex.Message}");
            }
        }

        Log.Error("✅ Finished processing outbox events.");
    }
}
