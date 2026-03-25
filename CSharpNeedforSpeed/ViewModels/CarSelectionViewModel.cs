using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpNeedforSpeed.Models;

namespace CSharpNeedforSpeed.ViewModels
{
    /// <summary>
    /// ViewModel for the car selection screen. Allows the player to browse
    /// available cars, view their stats, and start a race with the selected car.
    /// </summary>
    public partial class CarSelectionViewModel : ObservableObject
    {
        /// <summary>
        /// Gets the collection of available cars for the player to choose from.
        /// </summary>
        public ObservableCollection<Car> AvailableCars { get; }

        /// <summary>
        /// Gets or sets the currently selected car. When changed, updates the car details display.
        /// </summary>
        [ObservableProperty]
        private Car? _selectedCar;

        /// <summary>
        /// Gets or sets the formatted multi-line string displaying the selected car's statistics.
        /// </summary>
        [ObservableProperty]
        private string _carDetails;

        /// <summary>
        /// Gets or sets the error or status message displayed to the user.
        /// </summary>
        [ObservableProperty]
        private string _statusMessage;

        /// <summary>
        /// Event raised when the user wants to start a race with the selected car.
        /// The event passes the created <see cref="RaceManager"/> instance.
        /// </summary>
        public event Action<RaceManager>? StartRaceRequested;

        /// <summary>
        /// Event raised when the user wants to go back to the main menu.
        /// </summary>
        public event Action? GoBackRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="CarSelectionViewModel"/> class.
        /// Populates the available cars with one instance of each car type.
        /// </summary>
        public CarSelectionViewModel()
        {
            AvailableCars = new ObservableCollection<Car>
            {
                new SportsCar(),
                new EcoCar(),
                new MuscleCar()
            };

            _carDetails = "Select a car to view its stats.";
            _statusMessage = string.Empty;
        }

        /// <summary>
        /// Called when the <see cref="SelectedCar"/> property changes.
        /// Updates the <see cref="CarDetails"/> display with the newly selected car's statistics.
        /// </summary>
        /// <param name="value">The newly selected car.</param>
        partial void OnSelectedCarChanged(Car? value)
        {
            if (value is null)
            {
                CarDetails = "Select a car to view its stats.";
                return;
            }

            CarDetails = $@"--- {value.Name} ---

  Max Speed:          {value.MaxSpeed} km/h
  Fuel Consumption:   {value.FuelConsumptionRate} L/turn
  Fuel Capacity:      {value.FuelCapacity} L
  Pit Stop Refuel:    {value.PitStopRefuelAmount} L
  Pit Stop Penalty:   {value.PitStopTimePenalty}s

  Speed Up Distance:  {value.CalculateDistanceGained(PlayerAction.SpeedUp) * 100:F0}% per turn
  Maintain Distance:  {value.CalculateDistanceGained(PlayerAction.MaintainSpeed) * 100:F0}% per turn

  Speed Up Fuel Cost: {value.CalculateFuelUsage(PlayerAction.SpeedUp):F1} L
  Maintain Fuel Cost: {value.CalculateFuelUsage(PlayerAction.MaintainSpeed):F1} L";

            StatusMessage = string.Empty;
        }

        /// <summary>
        /// Command that validates the car selection and starts the race.
        /// Creates a <see cref="RaceManager"/> with the selected car and a default track.
        /// </summary>
        [RelayCommand]
        private void StartRace()
        {
            if (SelectedCar is null)
            {
                StatusMessage = "Please select a car before starting the race!";
                return;
            }

            var track = new Track();
            var raceManager = new RaceManager(SelectedCar, track);
            StartRaceRequested?.Invoke(raceManager);
        }

        /// <summary>
        /// Command that navigates back to the main menu.
        /// </summary>
        [RelayCommand]
        private void GoBack()
        {
            GoBackRequested?.Invoke();
        }
    }
}
