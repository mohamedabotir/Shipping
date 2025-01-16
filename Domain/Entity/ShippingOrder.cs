using Common.Entity;
using Common.ValueObject;
using Domain.ValueObject;

namespace Domain.Entity;

public class ShippingOrder:AggregateRoot
{
    public ShippingOrder(Guid  guid,long orderId,User userAddress,PackageOrder packageOrder)
    {
        Guid = guid;
        Id = orderId;
        UserAddress = userAddress;
        PackageOrder = packageOrder;
    }

    public PackageOrder PackageOrder { get;private set; }

    public User UserAddress { get; private set; }
    
}