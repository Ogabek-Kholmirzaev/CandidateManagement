using System.Text.Json;
using System.Text.Json.Serialization;

namespace CandidateManagement.Application.Converters;

public class TimeOnlyJsonConverter : JsonConverter<TimeOnly?>
{
    private const string Format = "HH:mm";

    public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (TimeOnly.TryParseExact(value, Format, out var result))
        {
            return result;
        }

        throw new JsonException($"Unable to convert \"{value}\" to TimeOnly. Expected format is {Format}.");
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString(Format));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
