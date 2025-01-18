using Infrastructure.Consumer.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class ShippingContextFactory
{
    private readonly Action<DbContextOptionsBuilder> _configureDbContext;

    public ShippingContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
    {
        _configureDbContext = configureDbContext;
    }

    public ShippingOrderContext CreateDataBaseContext()
    {

        DbContextOptionsBuilder<ShippingOrderContext> optionsBuilder = new();
        _configureDbContext(optionsBuilder);


        return new ShippingOrderContext(optionsBuilder.Options);

    }
}