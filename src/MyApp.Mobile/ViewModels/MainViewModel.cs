using CommunityToolkit.Mvvm.ComponentModel;
using MyApp.Mobile.ViewModels.Base;

namespace MyApp.Mobile.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to MyApp!";
}
