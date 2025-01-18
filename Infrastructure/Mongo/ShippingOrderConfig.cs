namespace Infrastructure.Mongo;

public class ShippingOrderConfig
{
    public string ConnectionString { get; init; }
    public string DatabaseName { get; init; }
    public string CollectionName { get; init; }
}