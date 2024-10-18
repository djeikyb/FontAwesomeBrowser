using System.Collections;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using fa.Vm;
using HotAvalonia;

namespace fa;

public class App : Application
{
    private static readonly Icons _icons;

    static App()
    {
        LogsSink = new ObservableLogEventSink(14);
        _icons = new Icons();
    }

    public static ObservableLogEventSink LogsSink { get; }

    public override void Initialize()
    {
        this.EnableHotReload(); // Ensure this line **precedes** `AvaloniaXamlLoader.Load(this);`
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new ViewModel(_icons)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
