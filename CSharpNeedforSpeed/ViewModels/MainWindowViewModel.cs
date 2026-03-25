using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CSharpNeedforSpeed.ViewModels
{
    /// <summary>
    /// ViewModel for the main window of the C# Speed Rush application.
    /// Provides commands for navigating to the car selection screen and
    /// viewing game instructions.
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether the instructions overlay is visible.
        /// </summary>
        [ObservableProperty]
        private bool _showInstructions;

        /// <summary>
        /// Gets the game instructions text displayed in the about/instructions overlay.
        /// </summary>
        [ObservableProperty]
        private string _instructionsText;

        /// <summary>
        /// Event raised when the user wants to navigate to the car selection screen.
        /// </summary>
        public event Action? NavigateToCarSelection;

        /// <summary>
        /// Event raised when the user wants to quit the application.
        /// </summary>
        public event Action? QuitRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            _showInstructions = false;
            _instructionsText = @"HOW TO PLAY C# NEED FOR SPEED
=================================

1. Select one of 3 unique cars, each with different
   speed, fuel, and pit stop characteristics.

2. Race over 5 laps on the Speed Rush Circuit.

3. Each turn, choose one of three actions:
   - Speed Up: Go faster, gain more distance, burn more fuel
   - Maintain Speed: Steady pace, balanced fuel usage
   - Pit Stop: Refuel your car (time penalty, no distance)

4. The race ends when you:
   - Complete all 5 laps (Victory!)
   - Run out of fuel (Game Over)
   - Run out of time (Game Over)

STRATEGY TIP: Balance speed with fuel management.
Pit stops cost time but keep you in the race!";
        }

        /// <summary>
        /// Command that navigates to the car selection screen to begin a new race.
        /// </summary>
        [RelayCommand]
        private void StartRace()
        {
            NavigateToCarSelection?.Invoke();
        }

        /// <summary>
        /// Command that toggles the visibility of the instructions overlay.
        /// </summary>
        [RelayCommand]
        private void ToggleInstructions()
        {
            ShowInstructions = !ShowInstructions;
        }

        /// <summary>
        /// Command that requests the application to quit.
        /// </summary>
        [RelayCommand]
        private void Quit()
        {
            QuitRequested?.Invoke();
        }
    }
}
