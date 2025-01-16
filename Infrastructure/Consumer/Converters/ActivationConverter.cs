using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Constants;
using Common.ValueObject;

namespace Infrastructure.Consumer.Converters;

public class ActivationConverter : JsonConverter<ActivationStatus>
{
    public override ActivationStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
        var value = jsonObject.GetProperty("ActivationStatus").GetInt32();
        var result = (ActivationStatus)value;
        

        return result;
    }

    public override void Write(Utf8JsonWriter writer, ActivationStatus value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}