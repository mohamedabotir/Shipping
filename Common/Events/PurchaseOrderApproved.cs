using System.Text.Json.Serialization;
using Common.Constants;
using Common.ValueObject;

namespace Common.Events;


public class PurchaseOrderApproved : DomainEventBase
{
    [ JsonConstructor]
    public PurchaseOrderApproved(Guid purchaseOrderId,
        string purchaseOrderNumber,
        ActivationStatus activationStatus,
        Money totalAmount,
        string customerName,
        string customerAddress,
        string customerPhoneNumber,
        PurchaseOrderStage orderStage ) : base(nameof(PurchaseOrderApproved))
    {
        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderNumber = purchaseOrderNumber;
        ActivationStatus = activationStatus;
        TotalAmount = totalAmount;
        CustomerName = customerName;
        CustomerAddress = customerAddress;
        CustomerPhoneNumber = customerPhoneNumber;
        OrderStage = orderStage;
    }

    public Guid PurchaseOrderId { get; }
    public string PurchaseOrderNumber { get;  }
    public ActivationStatus ActivationStatus { get; }
    public Money TotalAmount { get;}
    public string CustomerName { get;}
    public string CustomerAddress { get;}
    public string CustomerPhoneNumber { get;}
    public PurchaseOrderStage OrderStage { get;}
}