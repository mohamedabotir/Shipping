using Infrastructure.Consumer.Context.Pocos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class ShippingOrderContext : DbContext
{
    public ShippingOrderContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ShippingOrderPoco> ShippingOrder { get;  set; }
}