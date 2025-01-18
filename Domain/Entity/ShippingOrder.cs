using Common.Constants;
using Common.Entity;
using Common.Events;
using Common.Result;
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

    public Result ShipOrder()
    {
        if(PackageOrder.OrderStage != PurchaseOrderStage.Approved)
           return Result.Fail("Order on approved stage to Start Shipping it.");
        PackageOrder.OrderStage = PurchaseOrderStage.BeingShipped;
        AddDomainEvent(new OrderBeingShipped(Guid,PackageOrder.PurchaseOrderNumber));
        return Result.Ok();
    }

    public User Customer { get; private set; }
    
}