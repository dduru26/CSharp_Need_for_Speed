using Avalonia;

namespace CSharpNeedforSpeed
{
    /// <summary>
    /// Application entry point for the C# Speed Rush desktop application.
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// The main entry point for the application. Initialises and runs the Avalonia UI framework.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        /// <summary>
        /// Builds and configures the Avalonia application with the Fluent theme and default font.
        /// </summary>
        /// <returns>An <see cref="AppBuilder"/> configured for the C# Speed Rush application.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
