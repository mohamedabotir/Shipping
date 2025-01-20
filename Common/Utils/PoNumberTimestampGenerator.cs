namespace Common.Utils;

public class PoNumberTimestampGenerator : NumberGeneratorBase
{

    private readonly PoNumberPart[] _generatorPart =
    [
        new PoNumberPart(1, "PO",2),
        new PoNumberPart(2, "{",1),
        new PoNumberPart(3, $"{DateTime.Now:yyyyMMdd-HHmmss}",15),
        new PoNumberPart(4, "}",1),
    ];
     
    public override string GenerateNumber()
    {
        var number = string.Concat(_generatorPart.OrderBy(e=>e.OrderPart)
            .Select(e=>e.PartValue));
        return number;
    }

    public override bool IsValidNumber(string number)
    {
        int currentIndex = 0;
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
                if (substring != part.PartValue)
                {
                    return false;
                }
                currentIndex += part.Length;
            }
        }

        return currentIndex == number.Length;
    }
}