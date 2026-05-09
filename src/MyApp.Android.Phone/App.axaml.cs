using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MyApp.Mobile.Bootstrap;

namespace MyApp.Android.Phone;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        MobileAppSetup.Configure(ApplicationLifetime!, FormFactor.Phone);
        base.OnFrameworkInitializationCompleted();
    }
}
