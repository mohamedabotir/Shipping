using System.Text.Json.Serialization;

namespace Common.Events;

public class OrderBeingShipped: DomainEventBase
{
    [ JsonConstructor]
    public OrderBeingShipped(Guid purchaseOrderGuid,string poNumber):base(nameof(OrderBeingShipped))
    {
        PoNumber = poNumber;
        PurchaseOrderGuid = purchaseOrderGuid;
    }

    public string PoNumber { get; set; }
    public Guid PurchaseOrderGuid { get; set; }
}