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
           return Result.Fail("Order should be on approved stage to Start Shipping it.");
        PackageOrder = new PackageOrder(PackageOrder.TotalAmount, PackageOrder.ActivationStatus,
            PackageOrder.PurchaseOrderNumber,
            PurchaseOrderStage.BeingShipped, PackageOrder.PurchaseOrderGuid);
        AddDomainEvent(new OrderBeingShipped(PackageOrder.PurchaseOrderGuid,PackageOrder.PurchaseOrderNumber));
        return Result.Ok();
    }
    public Result MarkOrderAsShipped()
    {
        if(PackageOrder.OrderStage != PurchaseOrderStage.BeingShipped)
            return Result.Fail("Order should be on BeingShipped stage to finish  shipment .");
        PackageOrder = new PackageOrder(PackageOrder.TotalAmount, PackageOrder.ActivationStatus,
            PackageOrder.PurchaseOrderNumber,
            PurchaseOrderStage.Shipped, PackageOrder.PurchaseOrderGuid);
        AddDomainEvent(new OrderShipped(PackageOrder.PurchaseOrderGuid,PackageOrder.PurchaseOrderNumber));
        return Result.Ok();
    }

    public Result MarkOrderAsDelivered()
    {
        if(PackageOrder.OrderStage != PurchaseOrderStage.Shipped)
            return Result.Fail("Order should be on Shipped stage to close  shipment .");
        PackageOrder = new PackageOrder(PackageOrder.TotalAmount, PackageOrder.ActivationStatus,
            PackageOrder.PurchaseOrderNumber,
            PurchaseOrderStage.Closed, PackageOrder.PurchaseOrderGuid);
        return Result.Ok();
    }
    public User Customer { get; private set; }
    
}