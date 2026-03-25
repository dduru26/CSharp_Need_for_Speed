using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpNeedforSpeed.Models;

namespace CSharpNeedforSpeed.ViewModels
{
    /// <summary>
    /// ViewModel for the race window. Manages the race state, player actions,
    /// and all display properties bound to the race UI including fuel, time,
    /// speed, progress, and event logging.
    /// </summary>
    public partial class RaceWindowViewModel : ObservableObject
    {
        /// <summary>
        /// The race manager instance controlling the game logic.
        /// </summary>
        private readonly RaceManager _raceManager;

        /// <summary>
        /// Gets the display string for the current lap (e.g., "Lap 2 / 5").
        /// </summary>
        [ObservableProperty]
        private string _lapDisplay;

        /// <summary>
        /// Gets the fuel level as a value from 0 to 100 for binding to a ProgressBar.
        /// </summary>
        [ObservableProperty]
        private double _fuelLevel;

        /// <summary>
        /// Gets the fuel percentage as a formatted string for display labels.
        /// </summary>
        [ObservableProperty]
        private double _fuelPercentage;

        /// <summary>
        /// Gets the raw remaining time in seconds.
        /// </summary>
        [ObservableProperty]
        private double _timeRemaining;

        /// <summary>
        /// Gets the remaining time as a percentage (0–100) for binding to a ProgressBar.
        /// </summary>
        [ObservableProperty]
        private double _timePercentage;

        /// <summary>
        /// Gets the current speed status display string.
        /// </summary>
        [ObservableProperty]
        private string _speedStatus;

        /// <summary>
        /// Gets the ASCII progress bar display showing lap progress.
        /// </summary>
        [ObservableProperty]
        private string _progressBarDisplay;

        /// <summary>
        /// Gets the event log display string (last 5 events joined by newlines).
        /// </summary>
        [ObservableProperty]
        private string _eventLogDisplay;

        /// <summary>
        /// Gets the status message shown to the player (coloured based on race state).
        /// </summary>
        [ObservableProperty]
        private string _statusMessage;

        /// <summary>
        /// Gets a value indicating whether player action buttons are enabled.
        /// </summary>
        [ObservableProperty]
        private bool _actionsEnabled;

        /// <summary>
        /// Gets a value indicating whether the Speed Up button is enabled.
        /// </summary>
        [ObservableProperty]
        private bool _speedUpEnabled;

        /// <summary>
        /// Gets a value indicating whether the Maintain Speed button is enabled.
        /// </summary>
        [ObservableProperty]
        private bool _maintainEnabled;

        /// <summary>
        /// Gets a value indicating whether the Pit Stop button is enabled.
        /// </summary>
        [ObservableProperty]
        private bool _pitStopEnabled;

        /// <summary>
        /// Gets a value indicating whether the race has ended (for UI state changes).
        /// </summary>
        [ObservableProperty]
        private bool _isRaceOver;

        /// <summary>
        /// Gets the race summary text displayed when the race ends.
        /// </summary>
        [ObservableProperty]
        private string _raceSummary;

        /// <summary>
        /// Gets a value indicating whether the status message represents a positive outcome.
        /// </summary>
        [ObservableProperty]
        private bool _isPositiveStatus;

        /// <summary>
        /// Gets the name of the car being raced, for display in the UI header.
        /// </summary>
        [ObservableProperty]
        private string _carName;

        /// <summary>
        /// Gets the name of the track, for display in the UI header.
        /// </summary>
        [ObservableProperty]
        private string _trackName;

        /// <summary>
        /// Event raised when the player wants to return to the main menu.
        /// </summary>
        public event Action? ReturnToMenuRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="RaceWindowViewModel"/> class
        /// with the specified race manager.
        /// </summary>
        /// <param name="raceManager">The race manager controlling the game logic.</param>
        public RaceWindowViewModel(RaceManager raceManager)
        {
            _raceManager = raceManager;
            _raceManager.StartRace();

            _carName = _raceManager.ActiveCar.Name;
            _trackName = _raceManager.ActiveTrack.Name;
            _lapDisplay = string.Empty;
            _speedStatus = string.Empty;
            _progressBarDisplay = string.Empty;
            _eventLogDisplay = string.Empty;
            _statusMessage = string.Empty;
            _raceSummary = string.Empty;

            UpdateAllProperties();
        }

        /// <summary>
        /// Command that processes a Speed Up action for the current turn.
        /// </summary>
        [RelayCommand]
        private void SpeedUp()
        {
            ProcessAction(PlayerAction.SpeedUp);
        }

        /// <summary>
        /// Command that processes a Maintain Speed action for the current turn.
        /// </summary>
        [RelayCommand]
        private void MaintainSpeed()
        {
            ProcessAction(PlayerAction.MaintainSpeed);
        }

        /// <summary>
        /// Command that processes a Pit Stop action for the current turn.
        /// </summary>
        [RelayCommand]
        private void PitStop()
        {
            ProcessAction(PlayerAction.PitStop);
        }

        /// <summary>
        /// Command that returns to the main menu.
        /// </summary>
        [RelayCommand]
        private void ReturnToMenu()
        {
            ReturnToMenuRequested?.Invoke();
        }

        /// <summary>
        /// Processes a player action by calling the race manager and updating all UI properties.
        /// Catches exceptions and displays user-friendly error messages.
        /// </summary>
        /// <param name="action">The player action to process.</param>
        private void ProcessAction(PlayerAction action)
        {
            try
            {
                var result = _raceManager.ProcessTurn(action);
                StatusMessage = result.StatusMessage;
                IsPositiveStatus = result.State == RaceState.InProgress || result.State == RaceState.Finished;

                if (result.State != RaceState.InProgress)
                {
                    HandleRaceEnd(result);
                }

                UpdateAllProperties();
            }
            catch (InvalidOperationException ex)
            {
                StatusMessage = ex.Message;
                IsPositiveStatus = false;
            }
        }

        /// <summary>
        /// Handles the end of the race by generating a summary and disabling actions.
        /// </summary>
        /// <param name="result">The final race result.</param>
        private void HandleRaceEnd(RaceResult result)
        {
            IsRaceOver = true;
            ActionsEnabled = false;
            SpeedUpEnabled = false;
            MaintainEnabled = false;
            PitStopEnabled = false;

            double totalTimeUsed = _raceManager.TotalTime - _raceManager.TimeRemaining;
            int totalTurns = _raceManager.TurnNumber;
            int lapsCompleted = Math.Min(_raceManager.ActiveTrack.CurrentLap - 1, _raceManager.ActiveTrack.TotalLaps);
            if (_raceManager.ActiveTrack.IsRaceComplete())
            {
                lapsCompleted = _raceManager.ActiveTrack.TotalLaps;
            }

            string outcomeText = result.State switch
            {
                RaceState.Finished => "VICTORY! Race Complete!",
                RaceState.OutOfFuel => "GAME OVER - Out of Fuel!",
                RaceState.OutOfTime => "GAME OVER - Out of Time!",
                _ => "Race Ended"
            };

            RaceSummary = $@"{outcomeText}

Car:            {_raceManager.ActiveCar.Name}
Track:          {_raceManager.ActiveTrack.Name}
Laps Completed: {lapsCompleted} / {_raceManager.ActiveTrack.TotalLaps}
Total Turns:    {totalTurns}
Time Used:      {totalTimeUsed:F1}s
Fuel Remaining: {_raceManager.ActiveCar.CurrentFuel:F1}L";
        }

        /// <summary>
        /// Called by an external timer tick (once per second) to update the countdown.
        /// </summary>
        /// <returns><c>true</c> if the race ended due to time expiring; otherwise, <c>false</c>.</returns>
        public bool OnTimerTick()
        {
            if (_raceManager.State != RaceState.InProgress) return false;

            bool expired = _raceManager.TickOneSecond();
            TimeRemaining = _raceManager.TimeRemaining;
            TimePercentage = (_raceManager.TimeRemaining / _raceManager.TotalTime) * 100.0;

            if (expired)
            {
                var result = new RaceResult(
                    false,
                    _raceManager.ActiveTrack.CurrentLap,
                    _raceManager.ActiveCar.CurrentFuel,
                    0,
                    RaceState.OutOfTime,
                    "Time's up! Race over.",
                    _raceManager.ActiveTrack.GetProgressBar()
                );
                HandleRaceEnd(result);
                StatusMessage = "Time's up! Race over.";
                IsPositiveStatus = false;
                UpdateAllProperties();
            }

            return expired;
        }

        /// <summary>
        /// Updates all bindable properties to reflect the current race manager state.
        /// </summary>
        private void UpdateAllProperties()
        {
            int displayLap = Math.Min(_raceManager.ActiveTrack.CurrentLap, _raceManager.ActiveTrack.TotalLaps);
            LapDisplay = $"Lap {displayLap} / {_raceManager.ActiveTrack.TotalLaps}";

            FuelLevel = (_raceManager.ActiveCar.CurrentFuel / _raceManager.ActiveCar.FuelCapacity) * 100.0;
            FuelPercentage = FuelLevel;

            TimeRemaining = _raceManager.TimeRemaining;
            TimePercentage = (_raceManager.TimeRemaining / _raceManager.TotalTime) * 100.0;

            SpeedStatus = $"Current Speed: {_raceManager.ActiveCar.CurrentSpeed:F0} km/h";

            ProgressBarDisplay = _raceManager.ActiveTrack.GetProgressBar();

            EventLogDisplay = string.Join("\n", _raceManager.EventLog);

            bool raceActive = _raceManager.State == RaceState.InProgress;
            ActionsEnabled = raceActive;
            SpeedUpEnabled = raceActive;
            MaintainEnabled = raceActive;

            if (raceActive)
            {
                double fuelPct = _raceManager.ActiveCar.CurrentFuel / _raceManager.ActiveCar.FuelCapacity;
                PitStopEnabled = fuelPct < 0.9;
            }
            else
            {
                PitStopEnabled = false;
            }
        }
    }
}
