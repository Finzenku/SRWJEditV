using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SRWJEditV.Attributes;
using SRWJEditV.ViewModels;
using SRWJEditV.Views;
using System.Linq;
using System.Reflection;

namespace SRWJEditV
{
    public partial class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow =  new MainWindow() 
                { 
                    DataContext = new MainWindowViewModel(
                        Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => t.GetCustomAttribute(typeof(EditorViewModelAttribute)) is not null)
                        .ToList()) 
                };
            base.OnFrameworkInitializationCompleted();
        }
    }
}
