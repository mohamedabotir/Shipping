using Common.Result;
using Common.Utils;
using Domain.Utils;

namespace Domain.ValueObject;
using  Common.ValueObject;
public class ShippingNumber: ValueObject<ShippingNumber>
{
    public string ShippingNumberValue { get;private set; }

    private ShippingNumber(string shippingNumber)
    {
        ShippingNumberValue = shippingNumber;
    }

    public static ShippingNumber CreateInstance(string purchaseNumber)
    {
        var shoNumber = new SHOGenerator(purchaseNumber)
            .GenerateNumber();
        return new ShippingNumber(shoNumber);
    } 
    protected override bool EqualsCore(ShippingNumber other)
    {
        return other.ShippingNumberValue == this.ShippingNumberValue;
    }

    protected override int GetHashCodeCore()
    {
        return this.ShippingNumberValue.GetHashCode();
    }
}