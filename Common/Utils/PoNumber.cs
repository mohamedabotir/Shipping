using Common.Result;
using Common.ValueObject;

namespace Common.Utils;

public class PoNumber:ValueObject<PoNumber>
{
    private PoNumber(string poNumberValue)
    {
        PoNumberValue = poNumberValue;
    }

    public static Result<PoNumber> CreateInstance(NumberGenerator numberGenerator)
    {
        NumberGeneratorBase numberGeneratorBase = NumberGeneratorBase.CreateGenerator(numberGenerator);
        var poNumber =(Maybe<string>) numberGeneratorBase.GenerateNumber();
        return poNumber.ToResult("PoNumber Cannot be null")
            .Ensure(e => e.Length > 10,"Po Must be Greater than 10")
            .Map(x=>new PoNumber(x));
    }

    public static Result<PoNumber> SetPoNumber(string poNumberValue)
    {
        var number = Result.Result.Fail<PoNumber>("Invalid PoNumber");
        foreach (int enumValue in Enum.GetValues(typeof(NumberGenerator)))
        {
            var generator = NumberGeneratorBase.CreateGenerator((NumberGenerator)enumValue);
            if(generator.IsValidNumber(poNumberValue)) 
                return new Result<PoNumber>(new PoNumber(poNumberValue),true,string.Empty);
        }
        return number;  
    }
    public string PoNumberValue { get; }
    protected override bool EqualsCore(PoNumber other)
    {
        return other.PoNumberValue == PoNumberValue;
    }

    protected override int GetHashCodeCore()
    {
        return GetHashCode();
    }
    
}