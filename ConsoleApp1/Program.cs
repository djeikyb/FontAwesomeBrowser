using System.Text.Json;
using System.Text.Json.Serialization;
using fa.Data;

namespace ConsoleApp1;

class Program
{
        [Flags, JsonConverter(typeof(JsonStringEnumConverter))]
        enum MyEnum
        {
            Value1 = 1,
            [JsonStringEnumMemberName("v2")]
            Value2 = 2,
        }
    static void Main(string[] args)
    {
        Console.WriteLine(JsonSerializer.Serialize(MyEnum.Value1 | MyEnum.Value2)); // "Value1, Custom enum value"


        var fs = File.OpenRead("/Users/jacob/fa/fontawesome-pro-6.5.2-desktop/metadata/icons.json");

        var got = new IconsSerde().Deserialize(fs);
        Console.WriteLine($"🥑 {got.Length}\n{got[0].Svg.Regular.Path}");
    }
}
