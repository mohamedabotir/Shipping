using Common.Entity;
using Common.ValueObject;
using Domain.ValueObject;

namespace Domain.Entity;

public class ShippingOrder:AggregateRoot
{
    public ShippingOrder(Guid  guid,long orderId,User customer,PackageOrder packageOrder)
    {
        Guid = guid;
        Id = orderId;
        Customer = customer;
        PackageOrder = packageOrder;
    }

    public PackageOrder PackageOrder { get;private set; }

    public User Customer { get; private set; }
    
}