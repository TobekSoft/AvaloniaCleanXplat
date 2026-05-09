using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MyApp.Desktop.ViewModels.Base;

namespace MyApp.Desktop;

[RequiresUnreferencedCode("ViewLocator uses reflection to resolve views from MyApp.Views.Desktop.")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null) return null;

        // Maps MyApp.Desktop.ViewModels.XxxViewModel → MyApp.Views.Desktop.Pages.XxxView
        var viewName = param.GetType().FullName!
            .Replace(".Desktop.ViewModels.", ".Views.Desktop.Pages.", StringComparison.Ordinal)
            .Replace("ViewModel", "View", StringComparison.Ordinal);

        var type = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetType(viewName))
            .FirstOrDefault(t => t is not null);

        if (type is not null)
            return (Control)Activator.CreateInstance(type)!;

        return new TextBlock { Text = $"View not found: {viewName}" };
    }

    public bool Match(object? data) => data is ViewModelBase;
}
