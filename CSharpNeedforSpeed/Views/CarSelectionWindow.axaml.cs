using Avalonia.Controls;
using CSharpNeedforSpeed.Models;
using CSharpNeedforSpeed.ViewModels;

namespace CSharpNeedforSpeed.Views
{
    /// <summary>
    /// Code-behind for the car selection window.
    /// Handles navigation events for starting a race or going back to the main menu.
    /// </summary>
    public partial class CarSelectionWindow : Window
    {
        /// <summary>
        /// The ViewModel instance for this window.
        /// </summary>
        private readonly CarSelectionViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CarSelectionWindow"/> class.
        /// Sets up the DataContext and subscribes to navigation events.
        /// </summary>
        public CarSelectionWindow()
        {
            InitializeComponent();
            _viewModel = new CarSelectionViewModel();
            DataContext = _viewModel;

            _viewModel.StartRaceRequested += OnStartRaceRequested;
            _viewModel.GoBackRequested += OnGoBackRequested;
        }

        /// <summary>
        /// Handles the start race event by opening the race window with the provided race manager.
        /// </summary>
        /// <param name="raceManager">The configured race manager for the new race.</param>
        private void OnStartRaceRequested(RaceManager raceManager)
        {
            var raceWindow = new RaceWindow(raceManager);
            raceWindow.Closed += (_, _) => this.Close();
            this.Hide();
            raceWindow.Show();
        }

        /// <summary>
        /// Handles the go back event by closing this window (returns to main menu).
        /// </summary>
        private void OnGoBackRequested()
        {
            Close();
        }
    }
}
