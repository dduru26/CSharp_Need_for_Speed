namespace CSharpNeedforSpeed.Models
{
    /// <summary>
    /// Manages the core game loop for a C# Speed Rush race. Handles turn processing,
    /// fuel management, time tracking, lap progression, and event logging.
    /// </summary>
    public class RaceManager
    {
        /// <summary>
        /// Gets the car currently being raced.
        /// </summary>
        public Car ActiveCar { get; }

        /// <summary>
        /// Gets the track on which the race is taking place.
        /// </summary>
        public Track ActiveTrack { get; }

        /// <summary>
        /// Gets or sets the remaining time in seconds before the race ends due to timeout.
        /// </summary>
        public double TimeRemaining { get; set; }

        /// <summary>
        /// Gets the total time allotted for the race in seconds.
        /// </summary>
        public double TotalTime { get; }

        /// <summary>
        /// Gets or sets the current state of the race.
        /// </summary>
        public RaceState State { get; set; }

        /// <summary>
        /// Gets the history of all lap records recorded during the race.
        /// </summary>
        public List<LapRecord> LapHistory { get; }

        /// <summary>
        /// Gets the event log queue containing the most recent race events (max 5).
        /// </summary>
        public Queue<string> EventLog { get; }

        /// <summary>
        /// Gets the current turn number.
        /// </summary>
        public int TurnNumber { get; private set; }

        private const int MaxEventLogSize = 5;
        private const double BaseTimePerTurn = 15.0;
        private const double SpeedUpTime = 12.0;
        private const double MaintainTime = 15.0;
        private const double PitStopBaseTime = 15.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="RaceManager"/> class.
        /// </summary>
        /// <param name="selectedCar">The car selected by the player for the race.</param>
        /// <param name="track">The track on which the race will take place.</param>
        /// <param name="totalTimeSeconds">The total time in seconds allowed for the race (default: 300).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="selectedCar"/> or <paramref name="track"/> is null.</exception>
        public RaceManager(Car selectedCar, Track track, double totalTimeSeconds = 300.0)
        {
            ActiveCar = selectedCar ?? throw new ArgumentNullException(nameof(selectedCar));
            ActiveTrack = track ?? throw new ArgumentNullException(nameof(track));
            TotalTime = totalTimeSeconds;
            TimeRemaining = totalTimeSeconds;
            State = RaceState.NotStarted;
            LapHistory = new List<LapRecord>();
            EventLog = new Queue<string>();
            TurnNumber = 0;
        }

        /// <summary>
        /// Starts the race by setting the state to <see cref="RaceState.InProgress"/>
        /// and logging the start event.
        /// </summary>
        public void StartRace()
        {
            State = RaceState.InProgress;
            ActiveCar.CurrentSpeed = ActiveCar.MaxSpeed * 0.5;
            AddEventLog($"Race started! Driving {ActiveCar.Name} on {ActiveTrack.Name}.");
        }

        /// <summary>
        /// Processes a single turn of the race based on the player's chosen action.
        /// Updates fuel, distance, time, and race state accordingly.
        /// </summary>
        /// <param name="action">The action chosen by the player for this turn.</param>
        /// <returns>A <see cref="RaceResult"/> containing the outcome of the turn.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the race has not started, or when a pit stop is attempted
        /// while the fuel tank is already at 90% capacity or above.
        /// </exception>
        public RaceResult ProcessTurn(PlayerAction action)
        {
            if (State != RaceState.InProgress)
            {
                throw new InvalidOperationException("Race has not started.");
            }

            TurnNumber++;

            // Validate pit stop
            if (action == PlayerAction.PitStop)
            {
                double fuelPercentage = ActiveCar.CurrentFuel / ActiveCar.FuelCapacity;
                if (fuelPercentage >= 0.9)
                {
                    TurnNumber--;
                    throw new InvalidOperationException("Cannot pit stop: fuel tank is already at 90% or above.");
                }
            }

            // Calculate fuel usage
            double fuelUsed = ActiveCar.CalculateFuelUsage(action);
            ActiveCar.CurrentFuel -= fuelUsed;

            // Handle pit stop refuelling
            if (action == PlayerAction.PitStop)
            {
                double refuelAmount = ActiveCar.PitStopRefuelAmount;
                ActiveCar.CurrentFuel = Math.Min(ActiveCar.CurrentFuel + refuelAmount, ActiveCar.FuelCapacity);
            }

            // Check for out of fuel
            if (ActiveCar.CurrentFuel <= 0)
            {
                ActiveCar.CurrentFuel = 0;
                State = RaceState.OutOfFuel;
                string outOfFuelMsg = "Your car has run out of fuel! Race over.";
                AddEventLog(outOfFuelMsg);
                StoreLapRecord(fuelUsed, CalculateTimeForAction(action), action);
                return CreateResult(false, outOfFuelMsg);
            }

            // Calculate distance
            double distanceGained = ActiveCar.CalculateDistanceGained(action);
            bool lapCompleted = ActiveTrack.AdvanceLap(distanceGained);

            // Update speed display
            UpdateSpeed(action);

            // Deduct time
            double timeUsed = CalculateTimeForAction(action);
            TimeRemaining -= timeUsed;

            // Check for out of time
            if (TimeRemaining <= 0)
            {
                TimeRemaining = 0;
                State = RaceState.OutOfTime;
                string outOfTimeMsg = "Time's up! Race over.";
                AddEventLog(outOfTimeMsg);
                StoreLapRecord(fuelUsed, timeUsed, action);
                return CreateResult(lapCompleted, outOfTimeMsg);
            }

            // Check for race completion
            if (ActiveTrack.IsRaceComplete())
            {
                State = RaceState.Finished;
                double totalTimeUsed = TotalTime - TimeRemaining;
                string finishMsg = $"Race complete! Finished in {totalTimeUsed:F1}s!";
                AddEventLog(finishMsg);
                StoreLapRecord(fuelUsed, timeUsed, action);
                return CreateResult(true, finishMsg);
            }

            // Normal turn result
            string eventMsg = GenerateEventMessage(action, fuelUsed, distanceGained, lapCompleted);
            AddEventLog(eventMsg);
            StoreLapRecord(fuelUsed, timeUsed, action);

            string statusMsg = lapCompleted
                ? $"Lap {ActiveTrack.CurrentLap - 1} complete! Now on Lap {ActiveTrack.CurrentLap}."
                : $"Turn {TurnNumber}: {GetActionVerb(action)} — {distanceGained * 100:F0}% progress gained.";

            return CreateResult(lapCompleted, statusMsg);
        }

        /// <summary>
        /// Subtracts one second from the remaining time. Used by the UI timer tick.
        /// Updates the race state to OutOfTime if time runs out.
        /// </summary>
        /// <returns><c>true</c> if the race ended due to time expiring; otherwise, <c>false</c>.</returns>
        public bool TickOneSecond()
        {
            if (State != RaceState.InProgress) return false;

            TimeRemaining -= 1.0;
            if (TimeRemaining <= 0)
            {
                TimeRemaining = 0;
                State = RaceState.OutOfTime;
                AddEventLog("Time's up! Race over.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates the time cost in seconds for a given player action.
        /// </summary>
        /// <param name="action">The player action.</param>
        /// <returns>The time cost in seconds.</returns>
        private double CalculateTimeForAction(PlayerAction action)
        {
            return action switch
            {
                PlayerAction.SpeedUp => SpeedUpTime,
                PlayerAction.MaintainSpeed => MaintainTime,
                PlayerAction.PitStop => PitStopBaseTime + ActiveCar.PitStopTimePenalty,
                _ => BaseTimePerTurn
            };
        }

        /// <summary>
        /// Updates the car's current speed based on the player's action.
        /// </summary>
        /// <param name="action">The player action taken.</param>
        private void UpdateSpeed(PlayerAction action)
        {
            switch (action)
            {
                case PlayerAction.SpeedUp:
                    ActiveCar.CurrentSpeed = Math.Min(ActiveCar.CurrentSpeed + 20, ActiveCar.MaxSpeed);
                    break;
                case PlayerAction.MaintainSpeed:
                    break;
                case PlayerAction.PitStop:
                    ActiveCar.CurrentSpeed = Math.Max(ActiveCar.CurrentSpeed - 30, 0);
                    break;
            }
        }

        /// <summary>
        /// Generates a human-readable event message for the event log.
        /// </summary>
        /// <param name="action">The action taken.</param>
        /// <param name="fuelUsed">The fuel consumed.</param>
        /// <param name="distanceGained">The distance gained as a lap fraction.</param>
        /// <param name="lapCompleted">Whether a lap was completed.</param>
        /// <returns>A formatted event message string.</returns>
        private string GenerateEventMessage(PlayerAction action, double fuelUsed, double distanceGained, bool lapCompleted)
        {
            string actionText = GetActionVerb(action);
            string lapText = lapCompleted ? " [LAP COMPLETE!]" : "";
            return $"T{TurnNumber}: {actionText} | Fuel: -{fuelUsed:F1}L | +{distanceGained * 100:F0}%{lapText}";
        }

        /// <summary>
        /// Gets a human-readable verb for the given player action.
        /// </summary>
        /// <param name="action">The player action.</param>
        /// <returns>A string describing the action.</returns>
        private static string GetActionVerb(PlayerAction action)
        {
            return action switch
            {
                PlayerAction.SpeedUp => "Sped up",
                PlayerAction.MaintainSpeed => "Maintained speed",
                PlayerAction.PitStop => "Pit stop",
                _ => "Unknown action"
            };
        }

        /// <summary>
        /// Adds a message to the event log queue, removing the oldest entry if the queue exceeds the max size.
        /// </summary>
        /// <param name="message">The event message to log.</param>
        private void AddEventLog(string message)
        {
            EventLog.Enqueue(message);
            while (EventLog.Count > MaxEventLogSize)
            {
                EventLog.Dequeue();
            }
        }

        /// <summary>
        /// Stores a lap record in the lap history.
        /// </summary>
        /// <param name="fuelUsed">The fuel consumed during the turn.</param>
        /// <param name="timeUsed">The time used during the turn.</param>
        /// <param name="action">The action taken during the turn.</param>
        private void StoreLapRecord(double fuelUsed, double timeUsed, PlayerAction action)
        {
            LapHistory.Add(new LapRecord(ActiveTrack.CurrentLap, timeUsed, fuelUsed, action));
        }

        /// <summary>
        /// Creates a <see cref="RaceResult"/> from the current game state.
        /// </summary>
        /// <param name="lapCompleted">Whether a lap was completed this turn.</param>
        /// <param name="statusMessage">The status message for this result.</param>
        /// <returns>A new <see cref="RaceResult"/> reflecting the current state.</returns>
        private RaceResult CreateResult(bool lapCompleted, string statusMessage)
        {
            return new RaceResult(
                lapCompleted: lapCompleted,
                currentLap: ActiveTrack.CurrentLap,
                fuelRemaining: ActiveCar.CurrentFuel,
                timeRemaining: TimeRemaining,
                state: State,
                statusMessage: statusMessage,
                progressBar: ActiveTrack.GetProgressBar()
            );
        }
    }
}
