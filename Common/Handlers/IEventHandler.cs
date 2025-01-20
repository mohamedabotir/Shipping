namespace Common.Handlers;

public interface IEventHandler<T>
{
    Task HandleAsync (T @event, CancellationToken cancellationToken = default);
}