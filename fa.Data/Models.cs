using System.Text.Json.Serialization;

// When we gonna get a "c'mon man, this is a json dto" warning suppression..
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace fa.Data;

public class JsonIcon
{
    [JsonPropertyName("label")] public required string Label { get; set; }
    [JsonPropertyName("styles")] public required Styles Styles { get; set; }
    [JsonPropertyName("svg")] public required JsonSvgStyles Svg { get; set; }
    [JsonPropertyName("search")] public required JsonSearch Search { get; set; }
}

public class JsonSearch
{
    [JsonPropertyName("terms")] public List<string> terms { get; set; }
}

[Flags, JsonConverter(typeof(StylesArrayJsonConverter))]
public enum Styles : uint
{
    // it'd be so very annoying to not match the string casing used in the
    // actual json

    solid = 1 << 0,
    regular = 1 << 1,
    light = 1 << 2,
    thin = 1 << 3,
    brands = 1 << 4,
}

public class JsonSvgStyles
{
    [JsonPropertyName("solid")] public JsonSvg? Solid { get; set; }
    [JsonPropertyName("regular")] public JsonSvg? Regular { get; set; }
    [JsonPropertyName("light")] public JsonSvg? Light { get; set; }
    [JsonPropertyName("thin")] public JsonSvg? Thin { get; set; }
    [JsonPropertyName("brands")] public JsonSvg? Brands { get; set; }
}

public sealed class JsonSvg
{
    [JsonConstructor]
    internal JsonSvg()
    {
    }

    // This reference to the parent object should never be null, but I can't
    // prove it to the compiler. The guarantee of this dto lib is that after
    // deserializing the font awesome json, parent refs will be linked up before
    // passing the tree to the caller. And that's why this class is sealed and
    // prohibits construction outside of this lib.. To make sure callers (me)
    // don't violate the intent of the author (also me).
    [JsonIgnore] public JsonIcon Icon { get; set; } = null!;

    /// This will be a single value. Over in <see cref="JsonIcon.Styles"/> it
    /// indicates all styles supported. Here it indicates the style of this
    /// specific representation of an icon.
    [JsonIgnore]
    public Styles Style { get; set; }

    [JsonPropertyName("width")] public required int Width { get; set; }
    [JsonPropertyName("height")] public required int Height { get; set; }

    [JsonPropertyName("path")] public required string Path { get; set; }
}
