using System.Text.Json.Serialization;

namespace Common.Events;

public class OrderClosed: DomainEventBase
{
    [JsonConstructor]
    public OrderClosed(Guid purchaseOrderGuid, string poNumber) : base(nameof(OrderClosed))
    {
        PoNumber = poNumber;
        PurchaseOrderGuid = purchaseOrderGuid;
    }

    public string PoNumber { get; set; }
    public Guid PurchaseOrderGuid { get; set; }
}