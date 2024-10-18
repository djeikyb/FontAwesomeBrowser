using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace fa.Data;

public class StylesArrayJsonConverter : JsonConverter<Styles>
{
    public override Styles Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();
        Styles e = default;
        Span<char> chars = stackalloc char[8];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) break;
            if (reader.TokenType != JsonTokenType.String) throw new JsonException();

            // skip duotone cause i don't know how to render them yet
            if (reader.ValueSpan.SequenceEqual("duotone"u8)) continue;

            var status = Utf8.ToUtf16(reader.ValueSpan, chars, out _, out var written);
            if (status != OperationStatus.Done) throw new JsonException(status.ToString());
            e |= Enum.Parse<Styles>(chars.Slice(0, written));
        }

        return e;
    }

    public override void Write(Utf8JsonWriter writer, Styles value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
