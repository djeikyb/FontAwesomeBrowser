using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using fa.Vm;
using Serilog;
using Serilog.Events;

namespace fa.Views;

public partial class IconGrid : UserControl
{
    public IconGrid()
    {
        InitializeComponent();

        this.Tapped += (sender, args) =>
        {
            if (Log.IsEnabled(LogEventLevel.Debug))
            {
                Log.Debug($"⚡️ tap! {sender?.GetType()}");
            }

            var elements = this.GetInputElementsAt(args.GetPosition(this));
            foreach (var el in elements)
            {
                if (el is not StyledElement se) continue;
                if (se.DataContext is not JsonSvgVm j) continue;
                ((ViewModel)((IconGrid)sender!).DataContext!).IconListItemSelected.Value = j;
            }
        };
    }
}
