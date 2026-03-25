namespace CSharpNeedforSpeed.Models
{
    /// <summary>
    /// Represents a player action that can be taken each turn during the race.
    /// </summary>
    public enum PlayerAction
    {
        /// <summary>Increase speed to gain more distance but consume more fuel.</summary>
        SpeedUp,

        /// <summary>Maintain current speed for a balanced distance and fuel trade-off.</summary>
        MaintainSpeed,

        /// <summary>Make a pit stop to refuel the car at the cost of time.</summary>
        PitStop
    }

    /// <summary>
    /// Represents the current state of the race.
    /// </summary>
    public enum RaceState
    {
        /// <summary>The race has not yet started.</summary>
        NotStarted,

        /// <summary>The race is currently in progress.</summary>
        InProgress,

        /// <summary>The race has been completed successfully (all laps finished).</summary>
        Finished,

        /// <summary>The race ended because the car ran out of fuel.</summary>
        OutOfFuel,

        /// <summary>The race ended because the timer expired.</summary>
        OutOfTime
    }

    /// <summary>
    /// Stores information about a single lap during the race, including time spent,
    /// fuel consumed, and the action taken by the player.
    /// </summary>
    public struct LapRecord
    {
        /// <summary>
        /// Gets or sets the lap number this record corresponds to.
        /// </summary>
        public int LapNumber { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds used during this lap segment.
        /// </summary>
        public double TimeUsed { get; set; }

        /// <summary>
        /// Gets or sets the amount of fuel consumed during this lap segment.
        /// </summary>
        public double FuelUsed { get; set; }

        /// <summary>
        /// Gets or sets the player action taken during this lap segment.
        /// </summary>
        public PlayerAction Action { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LapRecord"/> struct.
        /// </summary>
        /// <param name="lapNumber">The lap number.</param>
        /// <param name="timeUsed">The time used in seconds.</param>
        /// <param name="fuelUsed">The fuel consumed in litres.</param>
        /// <param name="action">The player action taken.</param>
        public LapRecord(int lapNumber, double timeUsed, double fuelUsed, PlayerAction action)
        {
            LapNumber = lapNumber;
            TimeUsed = timeUsed;
            FuelUsed = fuelUsed;
            Action = action;
        }
    }
}
