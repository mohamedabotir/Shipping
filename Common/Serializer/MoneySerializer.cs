using System.Text.Json;
using System.Text.Json.Serialization;
using Common.ValueObject;

namespace Common.Serializer;

public class MoneyJsonConverter : JsonConverter<Money>
{
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
        var value = jsonObject.GetProperty("MoneyValue").GetDecimal();
        var result = Money.CreateInstance(value);
        if (!result.IsSuccess)
        {
            throw new JsonException($"Invalid Money value: {value}");
        }
        return result.Value;
    }

    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.MoneyValue);
    }
}