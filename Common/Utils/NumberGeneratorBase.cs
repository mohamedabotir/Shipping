namespace Common.Utils;

public abstract class NumberGeneratorBase
{
    public abstract string GenerateNumber();

    public static NumberGeneratorBase CreateGenerator(NumberGenerator generator) => generator switch
    {
        NumberGenerator.PoAndyymmdd => new PoNumberDateGenerator(),
        NumberGenerator.PoTimestamp => new PoNumberTimestampGenerator(),
        NumberGenerator.Sku => new SkuNumberGenerator(),
        _ => throw new ArgumentOutOfRangeException(nameof(generator), generator, null)
    };
    public abstract bool IsValidNumber(string number);
}