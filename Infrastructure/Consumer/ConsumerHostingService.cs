using Infrastructure.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Consumer;

public class ConsumerHostingService(ILogger<ConsumerHostingService> _logger,IServiceProvider _serviceProvider,IOptions<Topic> _topic):IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Event Consumer Event Running.");

        using (var scoped = _serviceProvider.CreateScope())
        {
            var consumer = scoped.ServiceProvider.GetRequiredService<IEventConsumer<EventConsumer>>();
            Task.Run(() => consumer.Consume(_topic.Value.TopicName), cancellationToken);

        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}