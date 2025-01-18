using System.Text.Json.Serialization;

namespace Infrastructure.GraphQL.Models;

public class PurchaseOrderActivationStatus
{
    [JsonPropertyName("purchaseOrderByPurchaseOrderNumber")]
    public SerializeActivationStatus PurchaseOrderByPurchaseOrderNumber { get; set; }
}