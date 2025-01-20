using Common.ValueObject;

namespace Common.ValueObject;
public enum QuantityType
{
    Kilo,
    Gram,
    Tab
}
public sealed class Quantity : ValueObject<Quantity>
{
    public Quantity()
    {
        
    }

    public Quantity(int quantityQuantityValue,QuantityType quantityType)
    {
        QuantityValue = quantityQuantityValue;
        QuantityType = quantityType;
    }

    public static readonly Quantity Tab = new Quantity(100, QuantityType.Tab);
    public  int QuantityValue { get;}
    public  QuantityType QuantityType { get; }

   
    protected override bool EqualsCore(Quantity other)
    {
        return other.QuantityValue == QuantityValue && QuantityType == other.QuantityType;
    }

    protected override int GetHashCodeCore()
    {
        unchecked
        {
         
            return GetHashCode();
        }    
    }
    public static Quantity operator +(Quantity a, Quantity b)
    {
        if(a.QuantityType != b.QuantityType)
            throw new InvalidOperationException("UnMatched Type Error");
        return new Quantity(a.QuantityValue + b.QuantityValue,a.QuantityType);
    }

    public static Quantity operator *(Quantity a, int factor)
    {
        if(factor<=0)
            throw new InvalidOperationException("Factor Cannot be Less than Zero");
        return new Quantity(a.QuantityValue * factor,a.QuantityType);
    }
    public static Quantity operator -(Quantity a, Quantity b)
    {
        if(a.QuantityType != b.QuantityType)
            throw new InvalidOperationException("UnMatched Type Error");

        if (a.QuantityValue <= b.QuantityValue)
        {
            throw new InvalidOperationException("Resulting quantity cannot be negative.");
        }

        return new Quantity(a.QuantityValue - b.QuantityValue,a.QuantityType);
    }
}