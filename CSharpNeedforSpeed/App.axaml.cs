using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CSharpNeedforSpeed.Views;
using CSharpNeedforSpeed.ViewModels;

namespace CSharpNeedforSpeed
{
    /// <summary>
    /// The main Avalonia application class for C# Speed Rush.
    /// Handles application initialisation and main window creation.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initialises the application by loading AXAML resources.
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Called when the Avalonia framework has completed initialisation.
        /// Sets up the main window with its DataContext.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
