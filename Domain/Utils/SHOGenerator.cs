using Common.Utils;

namespace Domain.Utils;

public class SHOGenerator(string purchaseNumberPart) : NumberGeneratorBase
{
    private readonly PoNumberPart[] _generatorPart =
    [
        new PoNumberPart(1, "SHO",3),
        new PoNumberPart(2, "-",1),
        new PoNumberPart(3, $"{DateTime.Now:yyyyMMdd}",8),
        new PoNumberPart(4, "-",1),
        new PoNumberPart(5, purchaseNumberPart,purchaseNumberPart.Length)

    ];

    public override string GenerateNumber()
    {
        var number = string.Concat(_generatorPart.OrderBy(e => e.OrderPart).Select(e=>e.PartValue));
        return number;
    }

    public override bool IsValidNumber(string number)
    {
        int currentIndex = 0;
        try
        {

            foreach (var part in _generatorPart.OrderBy(e => e.OrderPart))
            {
                if (part.OrderPart == 3)
                {
                    var substring = number.Substring(currentIndex, part.Length);
                    if (DateTime.TryParse(substring, out _))
                    {
                        return false;
                    }
                    currentIndex += part.Length;
                }
                else
                {
                    var substring = number.Substring(currentIndex, part.Length);
                    if (substring.Length != part.PartValue.Length)
                    {
                        return false;
                    }
                    currentIndex += part.Length;
                }
            }
        }
        catch (ArgumentOutOfRangeException) {
            return false;
        }

        return currentIndex == number.Length;
    }
    
}