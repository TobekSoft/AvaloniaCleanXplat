using CommunityToolkit.Mvvm.ComponentModel;
using MyApp.Desktop.ViewModels.Base;

namespace MyApp.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to MyApp!";
}
