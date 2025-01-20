namespace Common.Utils;

public class SkuNumberGenerator: NumberGeneratorBase{
    public override string GenerateNumber()
    {
        
        string part1 = Guid.NewGuid().ToString().Substring(0, 3).ToUpper();
        string part2 = Guid.NewGuid().ToString().Substring(0, 3).ToUpper();
        string part3 = Guid.NewGuid().ToString().Substring(0, 3).ToUpper();
    var result = "SKU-"+part1+"-"+part2+"-"+part3;
    return result;
    }

    public override bool IsValidNumber(string number)
    {
        var result = number.Split("-");
        foreach (var part in result)
        {
            if (part.Length!=3)
            {
                return false;
            }
        }
        return number.Length==15;
    }
}