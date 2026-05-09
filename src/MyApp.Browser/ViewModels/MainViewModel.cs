using CommunityToolkit.Mvvm.ComponentModel;
using MyApp.Browser.ViewModels.Base;

namespace MyApp.Browser.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to MyApp!";
}
