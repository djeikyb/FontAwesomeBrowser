using System.Text.Json;
using System.Text.Json.Nodes;

namespace fa.Data;

public class IconsSerde
{
    public JsonIcon[] Deserialize(Stream stream)
    {
        // HRM couldn't we read like 8k chunks?
        //     this is a ~30MiB file!
        var ms = new MemoryStream(1024 * 1024 * 33);
        stream.CopyTo(ms);

        ReadOnlySpan<byte> data = ms.ToArray();

        var len = CountElements(data);
        var icons = new JsonIcon[len];

        var jr = new Utf8JsonReader(data);
        jr.Read();

        if (jr.TokenType != JsonTokenType.StartObject)
            throw new JsonException("expected json object");

        int i;
        for (i = 0; jr.Read(); i++)
        {
            switch (jr.TokenType)
            {
                case JsonTokenType.EndObject:
                    goto done;
                case JsonTokenType.PropertyName:
                    try
                    {
                        var ob = JsonSerializer.Deserialize<JsonIcon>(ref jr);
                        icons[i] = ob ?? throw new JsonException($"Node {i} parsed as null.");
                    }
                    catch (Exception e)
                    {
                        throw new JsonException($"Failed to parse node {i}\n{e}");
                    }

                    continue;
                default:
                    throw new JsonException($"""
                                             expected: {JsonTokenType.PropertyName}
                                              but was: {jr.TokenType}
                                             """);
            }
        }

        done:

        foreach (var icon in icons)
        {
            if (icon.Svg.Thin != null)
            {
                icon.Svg.Thin.Icon = icon;
                icon.Svg.Thin.Style = Styles.thin;
            }

            if (icon.Svg.Light != null)
            {
                icon.Svg.Light.Icon = icon;
                icon.Svg.Light.Style = Styles.light;
            }

            if (icon.Svg.Regular != null)
            {
                icon.Svg.Regular.Icon = icon;
                icon.Svg.Regular.Style = Styles.regular;
            }

            if (icon.Svg.Solid != null)
            {
                icon.Svg.Solid.Icon = icon;
                icon.Svg.Solid.Style = Styles.solid;
            }

            if (icon.Svg.Brands != null)
            {
                icon.Svg.Brands.Icon = icon;
                icon.Svg.Brands.Style = Styles.brands;
            }
        }

        return icons;
    }

    public int CountElements(ReadOnlySpan<byte> data)
    {
        int i = 0;
        try
        {
            var jr = new Utf8JsonReader(data);
            jr.Read();

            if (jr.TokenType != JsonTokenType.StartObject)
                throw new Exception($"""
                                     expected: {JsonTokenType.StartObject}
                                      but was: {jr.TokenType}
                                     """);

            for (; jr.Read(); i++)
            {
                switch (jr.TokenType)
                {
                    case JsonTokenType.EndObject:
                        goto done;
                    case JsonTokenType.PropertyName:
                        // HRM benchmark skip vs parse?
                        jr.Skip();
                        // var _ = JsonNode.Parse(ref jr);
                        continue;
                    default:
                        throw new Exception($"""
                                             expected: {JsonTokenType.PropertyName}
                                              but was: {jr.TokenType}
                                             """);
                }
            }

            done:
            return i;
        }
        catch (Exception e)
        {
            Console.WriteLine($"iter {i}.\n{e}");
            return -1;
        }
    }
}
