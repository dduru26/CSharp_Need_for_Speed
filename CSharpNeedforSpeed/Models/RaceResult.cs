namespace CSharpNeedforSpeed.Models
{
    /// <summary>
    /// Represents the result of processing a single turn in the race.
    /// Contains all relevant state information after the turn has been executed.
    /// </summary>
    public struct RaceResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether a lap was completed during this turn.
        /// </summary>
        public bool LapCompleted { get; set; }

        /// <summary>
        /// Gets or sets the current lap number after the turn was processed.
        /// </summary>
        public int CurrentLap { get; set; }

        /// <summary>
        /// Gets or sets the remaining fuel in litres after the turn was processed.
        /// </summary>
        public double FuelRemaining { get; set; }

        /// <summary>
        /// Gets or sets the remaining time in seconds after the turn was processed.
        /// </summary>
        public double TimeRemaining { get; set; }

        /// <summary>
        /// Gets or sets the current state of the race after the turn was processed.
        /// </summary>
        public RaceState State { get; set; }

        /// <summary>
        /// Gets or sets a human-readable status message describing what happened during the turn.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Gets or sets an ASCII progress bar representing the current lap progress.
        /// </summary>
        public string ProgressBar { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RaceResult"/> struct with all fields.
        /// </summary>
        /// <param name="lapCompleted">Whether a lap was completed this turn.</param>
        /// <param name="currentLap">The current lap number.</param>
        /// <param name="fuelRemaining">The remaining fuel in litres.</param>
        /// <param name="timeRemaining">The remaining time in seconds.</param>
        /// <param name="state">The current race state.</param>
        /// <param name="statusMessage">A description of what happened.</param>
        /// <param name="progressBar">The ASCII progress bar string.</param>
        public RaceResult(bool lapCompleted, int currentLap, double fuelRemaining,
            double timeRemaining, RaceState state, string statusMessage, string progressBar)
        {
            LapCompleted = lapCompleted;
            CurrentLap = currentLap;
            FuelRemaining = fuelRemaining;
            TimeRemaining = timeRemaining;
            State = state;
            StatusMessage = statusMessage;
            ProgressBar = progressBar;
        }
    }
}
