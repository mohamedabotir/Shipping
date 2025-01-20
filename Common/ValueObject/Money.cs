using Common.Result;
using Common.ValueObject;

namespace Common.ValueObject;

public class Money:ValueObject<Money>
{
    public decimal MoneyValue { get; }

    private Money(decimal moneyValue)
    {
        MoneyValue = moneyValue;
    }

    public static Result<Money> CreateInstance(decimal money)
    {
        var moneyInstance = new Money(money);
        return new Result<Money>(moneyInstance, true, string.Empty)
            .Ensure(e => e.MoneyValue > 0, "Money Cannot be Zero Or Less Than Zero");
    }
    protected override bool EqualsCore(Money other)
    {
      return  other.MoneyValue==MoneyValue;
    }

    protected override int GetHashCodeCore()
    {
       return GetHashCode();
    }
}