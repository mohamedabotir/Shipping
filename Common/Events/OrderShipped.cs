using System.Text.Json.Serialization;

namespace Common.Events;

public class OrderShipped : DomainEventBase
{
    [JsonConstructor]
    public OrderShipped(Guid purchaseOrderGuid, string poNumber) : base(nameof(OrderShipped))
    {
        PoNumber = poNumber;
        PurchaseOrderGuid = purchaseOrderGuid;
    }

    public string PoNumber { get; set; }
    public Guid PurchaseOrderGuid { get; set; }
}