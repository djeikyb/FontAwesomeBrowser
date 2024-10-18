using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using fa.Data;

namespace fa.Views;

public static class Converters
{
    public static FuncValueConverter<fa.Data.Styles?, List<string>> StyleEnumConverter { get; } =
        new((Styles? maybeStyle
            ) =>
        {
            if (maybeStyle is not { } x) return [];
            var a = new string[BitOperations.PopCount((uint)x)];
            var i = -1;

            // The ¡bang! to suppress nulls is correct SO LONG as the
            // invariant holds in the original json from FontAwesome. For
            // every style in the "styles" array, there should be an object
            // of that style in the "svg" dictionary.
            if (x.HasFlag(Styles.solid)) a[++i] = Styles.solid.ToString();
            if (x.HasFlag(Styles.regular)) a[++i] = Styles.regular.ToString();
            if (x.HasFlag(Styles.light)) a[++i] = Styles.light.ToString();
            if (x.HasFlag(Styles.thin)) a[++i] = Styles.thin.ToString();
            if (x.HasFlag(Styles.brands)) a[++i] = Styles.brands.ToString();

            return a.ToList();
        });
}

public partial class IconDeets : UserControl
{
    public IconDeets()
    {
        InitializeComponent();
    }
}
