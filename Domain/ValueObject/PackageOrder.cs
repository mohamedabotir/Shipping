using Common.Constants;

namespace Domain.ValueObject;
using Common.ValueObject;
public class PackageOrder:ValueObject<PackageOrder>
{
    public Money TotalAmount { get;private set; }
    public ActivationStatus ActivationStatus { get; private set; }
    public string PurchaseOrderNumber { get; protected set; }
    public PurchaseOrderStage OrderStage { get; set; }

    public PackageOrder(Money totalAmount, ActivationStatus activationStatus,
        string purchaseOrderNumber,PurchaseOrderStage orderStage)
    {
        
        TotalAmount = totalAmount;
        ActivationStatus = activationStatus;
        PurchaseOrderNumber = purchaseOrderNumber;
        OrderStage = orderStage;
    }
    protected override bool EqualsCore(PackageOrder other)
    {
       return other.PurchaseOrderNumber == PurchaseOrderNumber;
    }

    protected override int GetHashCodeCore()
    {
        return HashCode.Combine(PurchaseOrderNumber, TotalAmount);
    }
}