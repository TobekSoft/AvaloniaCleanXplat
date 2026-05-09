using Avalonia.Controls.ApplicationLifetimes;
using PhoneMainView = MyApp.Mobile.Views.Phone.MainView;
using TabletMainView = MyApp.Mobile.Views.Tablet.MainView;
using MyApp.Mobile.ViewModels;

namespace MyApp.Mobile.Bootstrap;

public enum FormFactor { Phone, Tablet }

public static class MobileAppSetup
{
    public static void Configure(IApplicationLifetime lifetime, FormFactor formFactor)
    {
        if (lifetime is IActivityApplicationLifetime android)
        {
            android.MainViewFactory = formFactor == FormFactor.Phone
                ? () => new PhoneMainView { DataContext = new MainViewModel() }
                : () => new TabletMainView { DataContext = new MainViewModel() };
        }
        else if (lifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = formFactor == FormFactor.Phone
                ? new PhoneMainView { DataContext = new MainViewModel() }
                : new TabletMainView { DataContext = new MainViewModel() };
        }
    }
}
