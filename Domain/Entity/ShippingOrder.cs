using Common.Constants;
using Common.Entity;
using Common.Events;
using Common.Exceptions;
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
    public ShippingOrder()
    {
        
    }
    public static ShippingOrder CreateShippingOrder(Guid guid, long orderId, User customer, PackageOrder packageOrder, PurchaseOrderApproved @event) {
        var aggregate = new ShippingOrder(guid, orderId, customer, packageOrder);
        aggregate.RaiseEvent(@event);
        return aggregate;
    }
    public PackageOrder PackageOrder { get;private set; }

    public Result ShipOrder()
    {
        if(PackageOrder.OrderStage != PurchaseOrderStage.Approved)
           return Result.Fail("Order should be on approved stage to Start Shipping it.");
        PackageOrder = new PackageOrder(PackageOrder.TotalAmount, PackageOrder.ActivationStatus,
            PackageOrder.PurchaseOrderNumber,
            PurchaseOrderStage.BeingShipped, PackageOrder.PurchaseOrderGuid);
        RaiseEvent(new OrderBeingShipped(PackageOrder.PurchaseOrderGuid,PackageOrder.PurchaseOrderNumber, PackageOrder.TotalAmount.MoneyValue,
            PackageOrder.ActivationStatus,PackageOrder.PurchaseOrderNumber, PackageOrder.OrderStage,Guid,Id,Customer.Name,Customer.Address,Customer.PhoneNumber));
        return Result.Ok();
    }
    public Result MarkOrderAsShipped()
    {
        if(PackageOrder.OrderStage != PurchaseOrderStage.BeingShipped)
            return Result.Fail("Order should be on BeingShipped stage to finish  shipment .");
        PackageOrder = new PackageOrder(PackageOrder.TotalAmount, PackageOrder.ActivationStatus,
            PackageOrder.PurchaseOrderNumber,
            PurchaseOrderStage.Shipped, PackageOrder.PurchaseOrderGuid);
        RaiseEvent(new OrderShipped(PackageOrder.PurchaseOrderGuid,PackageOrder.PurchaseOrderNumber));
        return Result.Ok();
    }
    public void Apply(OrderBeingShipped @event) {

        var money = Money.CreateInstance(@event.TotalAmount);
        var customer = User.CreateInstance(@event.CustomerName, @event.CustomerAddress, @event.CustomerPhoneNumber);
        var result = Result.Combine(money, customer);
        if (result.IsFailure)
            throw new DomainException(result.Message);
        PackageOrder = new PackageOrder(money.Value, @event.ActivationStatus,
            @event.PurchaseOrderNumber,
            PurchaseOrderStage.BeingShipped, @event.PurchaseOrderGuid);
        Guid = @event.ShippingOrderGuid;
        Id = @event.ShippingOrderId;
        Customer = customer.Value;
    }    
    public void Apply(OrderShipped @event) {
        PackageOrder = new PackageOrder(PackageOrder.TotalAmount, PackageOrder.ActivationStatus, @event.PoNumber, PurchaseOrderStage.Shipped, @event.PurchaseOrderGuid);
    }

    public Result MarkOrderAsDelivered(OrderClosed orderClosedEvent)
    {
        if(PackageOrder.OrderStage != PurchaseOrderStage.Shipped)
            return Result.Fail("Order should be on Shipped stage to close  shipment .");
        PackageOrder = new PackageOrder(PackageOrder.TotalAmount, PackageOrder.ActivationStatus,
            PackageOrder.PurchaseOrderNumber,
            PurchaseOrderStage.Closed, PackageOrder.PurchaseOrderGuid);
        RaiseEvent(orderClosedEvent);
        return Result.Ok();
    }
    public void Apply(OrderClosed @event)
    {
        PackageOrder = new PackageOrder(PackageOrder.TotalAmount,PackageOrder.ActivationStatus,PackageOrder.PurchaseOrderNumber
            ,PurchaseOrderStage.Closed,PackageOrder.PurchaseOrderGuid);
    }

    public void Apply(PurchaseOrderApproved @event)
    {
        var address = Address.CreateInstance(@event.CustomerAddress);
        if (address.IsFailure)
            return;
        var user = User.CreateInstance(@event.CustomerName, address.Value, @event.CustomerPhoneNumber);
        if (user.IsFailure)
            return;
        var package = new PackageOrder(@event.TotalAmount, @event.ActivationStatus, @event.PurchaseOrderNumber, @event.OrderStage,
            @event.PurchaseOrderId);
        Customer = user.Value;
        PackageOrder = package;
    }
    public User Customer { get; private set; }
    
}