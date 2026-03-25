using Avalonia.Controls;
using CSharpNeedforSpeed.ViewModels;

namespace CSharpNeedforSpeed.Views
{
    /// <summary>
    /// Code-behind for the main window of the C# Speed Rush application.
    /// Handles navigation events from the ViewModel to open child windows.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The ViewModel instance for this window.
        /// </summary>
        private readonly MainWindowViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Sets up the DataContext and subscribes to navigation events.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            _viewModel.NavigateToCarSelection += OnNavigateToCarSelection;
            _viewModel.QuitRequested += OnQuitRequested;
        }

        /// <summary>
        /// Handles navigation to the car selection screen by hiding the main window
        /// and opening the car selection window.
        /// </summary>
        private void OnNavigateToCarSelection()
        {
            var carSelectionWindow = new CarSelectionWindow();
            carSelectionWindow.Closed += (_, _) => this.Show();
            this.Hide();
            carSelectionWindow.Show();
        }

        /// <summary>
        /// Handles the quit request by closing the main window and exiting the application.
        /// </summary>
        private void OnQuitRequested()
        {
            Close();
        }
    }
}
