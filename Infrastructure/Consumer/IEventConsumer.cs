namespace Infrastructure.Consumer;

public interface IEventConsumer<TSource> where TSource : class
{
    void Consume(string topic);

}