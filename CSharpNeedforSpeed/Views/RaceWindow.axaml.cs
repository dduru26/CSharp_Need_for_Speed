using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Threading;
using CSharpNeedforSpeed.Models;
using CSharpNeedforSpeed.ViewModels;

namespace CSharpNeedforSpeed.Views
{
    /// <summary>
    /// Code-behind for the race window. Manages the race timer and navigation events.
    /// </summary>
    public partial class RaceWindow : Window
    {
        /// <summary>
        /// The ViewModel instance for this window.
        /// </summary>
        private readonly RaceWindowViewModel _viewModel;

        /// <summary>
        /// Timer that ticks every second to count down the remaining race time.
        /// </summary>
        private readonly DispatcherTimer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RaceWindow"/> class.
        /// Required by Avalonia AXAML loader. Use the overload with RaceManager for actual gameplay.
        /// </summary>
        public RaceWindow() : this(new RaceManager(new SportsCar(), new Track()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RaceWindow"/> class
        /// with the specified race manager.
        /// </summary>
        /// <param name="raceManager">The race manager controlling the game logic.</param>
        public RaceWindow(RaceManager raceManager)
        {
            InitializeComponent();
            _viewModel = new RaceWindowViewModel(raceManager);
            DataContext = _viewModel;

            _viewModel.ReturnToMenuRequested += OnReturnToMenu;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        /// <summary>
        /// Handles the timer tick event by updating the ViewModel countdown.
        /// Stops the timer if the race has ended.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimerTick(object? sender, EventArgs e)
        {
            bool raceEnded = _viewModel.OnTimerTick();
            if (raceEnded)
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// Handles the return to menu event by stopping the timer and closing the window.
        /// </summary>
        private void OnReturnToMenu()
        {
            _timer.Stop();
            Close();
        }
    }

    /// <summary>
    /// Converts a boolean value to a colour brush for status message display.
    /// True (positive) maps to cyan/blue; false (negative) maps to red.
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        /// <summary>
        /// Singleton instance of the converter for use in AXAML bindings.
        /// </summary>
        public static readonly BoolToColorConverter Instance = new();

        /// <summary>
        /// Converts a boolean to a <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The target type (ignored).</param>
        /// <param name="parameter">An optional parameter (ignored).</param>
        /// <param name="culture">The culture info (ignored).</param>
        /// <returns>A green brush for true, red brush for false.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isPositive)
            {
                return isPositive
                    ? new SolidColorBrush(Color.Parse("#81ecff"))
                    : new SolidColorBrush(Color.Parse("#ff716c"));
            }
            return new SolidColorBrush(Color.Parse("#a5abb9"));
        }

        /// <summary>
        /// Not implemented. One-way binding only.
        /// </summary>
        /// <param name="value">The value to convert back.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">An optional parameter.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>Not supported.</returns>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
