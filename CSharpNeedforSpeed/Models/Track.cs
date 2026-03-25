namespace CSharpNeedforSpeed.Models
{
    /// <summary>
    /// Represents a racing track in the C# Speed Rush game.
    /// Manages lap progression, progress within the current lap,
    /// and provides visual progress display.
    /// </summary>
    public class Track
    {
        /// <summary>
        /// Gets the name of the track.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the total number of laps required to complete the race.
        /// </summary>
        public int TotalLaps { get; }

        /// <summary>
        /// Gets the distance of each lap in kilometres.
        /// </summary>
        public double LapDistance { get; }

        /// <summary>
        /// Gets or sets the current lap number (1-based). Starts at 1 and increments
        /// each time a full lap is completed.
        /// </summary>
        public int CurrentLap { get; set; }

        /// <summary>
        /// Gets or sets the progress within the current lap as a fraction from 0.0 to 1.0.
        /// When this reaches or exceeds 1.0, a lap is completed.
        /// </summary>
        public double LapProgress { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Track"/> class.
        /// </summary>
        /// <param name="name">The display name of the track.</param>
        /// <param name="totalLaps">The total number of laps for the race (default: 5).</param>
        /// <param name="lapDistance">The distance of each lap in kilometres (default: 5.0).</param>
        public Track(string name = "Speed Rush Circuit", int totalLaps = 5, double lapDistance = 5.0)
        {
            Name = name;
            TotalLaps = totalLaps;
            LapDistance = lapDistance;
            CurrentLap = 1;
            LapProgress = 0.0;
        }

        /// <summary>
        /// Advances the track progress by the specified amount. If the progress within
        /// the current lap reaches or exceeds 1.0, the current lap is incremented and
        /// the excess progress carries over.
        /// </summary>
        /// <param name="progressGained">The fraction of a lap gained (0.0 to 1.0+).</param>
        /// <returns><c>true</c> if a lap was completed during this advance; otherwise, <c>false</c>.</returns>
        public bool AdvanceLap(double progressGained)
        {
            LapProgress += progressGained;
            bool lapCompleted = false;

            while (LapProgress >= 1.0 && CurrentLap <= TotalLaps)
            {
                LapProgress -= 1.0;
                CurrentLap++;
                lapCompleted = true;
            }

            return lapCompleted;
        }

        /// <summary>
        /// Determines whether the race is complete by checking if all laps have been finished.
        /// </summary>
        /// <returns><c>true</c> if the current lap exceeds the total number of laps; otherwise, <c>false</c>.</returns>
        public bool IsRaceComplete()
        {
            return CurrentLap > TotalLaps;
        }

        /// <summary>
        /// Generates an ASCII progress bar representing the current progress within the active lap.
        /// </summary>
        /// <param name="width">The total character width of the progress bar (default: 20).</param>
        /// <returns>A string like <c>[=====>              ] 25%</c> showing current lap progress.</returns>
        public string GetProgressBar(int width = 20)
        {
            if (IsRaceComplete())
            {
                string fullBar = new string('=', width);
                return $"[{fullBar}] 100%";
            }

            double clampedProgress = Math.Clamp(LapProgress, 0.0, 1.0);
            int filledCount = (int)(clampedProgress * width);
            int emptyCount = width - filledCount;

            string filled = new string('=', filledCount);
            string arrow = filledCount < width ? ">" : "";
            string empty = new string(' ', Math.Max(0, emptyCount - (arrow.Length > 0 ? 1 : 0)));
            int percentage = (int)(clampedProgress * 100);

            return $"[{filled}{arrow}{empty}] {percentage}%";
        }

        /// <summary>
        /// Returns a string representation of the track including its name, lap count, and distance.
        /// </summary>
        /// <returns>A formatted string describing the track.</returns>
        public override string ToString()
        {
            return $"{Name} | {TotalLaps} Laps | {LapDistance} km per lap";
        }
    }
}
