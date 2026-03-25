using Xunit;
using CSharpNeedforSpeed.Models;

namespace CSharpNeedforSpeed.Tests
{
    /// <summary>
    /// Unit tests for the RaceManager class. Verifies the core game loop mechanics
    /// including turn processing, state transitions, and exception handling.
    /// </summary>
    public class RaceManagerTests
    {
        /// <summary>
        /// Creates a standard RaceManager with a SportsCar and default track for testing.
        /// </summary>
        /// <returns>A configured <see cref="RaceManager"/> instance.</returns>
        private static RaceManager CreateDefaultManager()
        {
            var car = new SportsCar();
            var track = new Track();
            return new RaceManager(car, track);
        }

        /// <summary>
        /// Verifies that calling ProcessTurn before StartRace throws an
        /// <see cref="InvalidOperationException"/> with the appropriate message.
        /// </summary>
        [Fact]
        public void ProcessTurn_BeforeStart_ThrowsInvalidOperationException()
        {
            // Arrange
            var manager = CreateDefaultManager();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                manager.ProcessTurn(PlayerAction.SpeedUp));
            Assert.Contains("not started", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Verifies that a SpeedUp action correctly reduces both time and fuel
        /// from their initial values.
        /// </summary>
        [Fact]
        public void ProcessTurn_SpeedUp_UpdatesTimeAndFuel()
        {
            // Arrange
            var manager = CreateDefaultManager();
            manager.StartRace();
            double initialFuel = manager.ActiveCar.CurrentFuel;
            double initialTime = manager.TimeRemaining;

            // Act
            var result = manager.ProcessTurn(PlayerAction.SpeedUp);

            // Assert
            Assert.True(result.FuelRemaining < initialFuel,
                "Fuel should decrease after SpeedUp action");
            Assert.True(result.TimeRemaining < initialTime,
                "Time should decrease after SpeedUp action");
            Assert.Equal(RaceState.InProgress, result.State);
        }

        /// <summary>
        /// Verifies that the race state transitions to OutOfFuel when the car
        /// runs out of fuel during a turn.
        /// </summary>
        [Fact]
        public void ProcessTurn_OutOfFuel_SetsCorrectRaceState()
        {
            // Arrange
            var car = new SportsCar();
            car.CurrentFuel = 1.0; // Almost empty
            var track = new Track();
            var manager = new RaceManager(car, track);
            manager.StartRace();

            // Act
            var result = manager.ProcessTurn(PlayerAction.SpeedUp); // Uses 6.75L, only 1L available

            // Assert
            Assert.Equal(RaceState.OutOfFuel, result.State);
            Assert.Equal(0, result.FuelRemaining);
        }

        /// <summary>
        /// Verifies that the race state transitions to Finished when all laps
        /// are completed during gameplay.
        /// </summary>
        [Fact]
        public void ProcessTurn_CompletesAllLaps_FinishesRace()
        {
            // Arrange - Use MuscleCar for fastest lap completion (0.30 per SpeedUp)
            var car = new MuscleCar();
            var track = new Track(totalLaps: 5);
            var manager = new RaceManager(car, track, totalTimeSeconds: 600);
            manager.StartRace();

            RaceResult result = default;
            int maxTurns = 50;

            // Act — keep speeding up until race ends or we hit max turns
            for (int i = 0; i < maxTurns && manager.State == RaceState.InProgress; i++)
            {
                // Refuel if low
                if (car.CurrentFuel < 15 && (car.CurrentFuel / car.FuelCapacity) < 0.9)
                {
                    result = manager.ProcessTurn(PlayerAction.PitStop);
                }
                else
                {
                    result = manager.ProcessTurn(PlayerAction.SpeedUp);
                }
            }

            // Assert
            Assert.Equal(RaceState.Finished, result.State);
        }

        /// <summary>
        /// Verifies that MaintainSpeed uses more time (15s) than SpeedUp (12s).
        /// </summary>
        [Fact]
        public void ProcessTurn_MaintainSpeed_UsesCorrectTime()
        {
            // Arrange
            var manager = CreateDefaultManager();
            manager.StartRace();
            double timeBefore = manager.TimeRemaining;

            // Act
            manager.ProcessTurn(PlayerAction.MaintainSpeed);
            double timeAfter = manager.TimeRemaining;
            double timeUsed = timeBefore - timeAfter;

            // Assert — MaintainSpeed should cost exactly 15 seconds
            Assert.Equal(15.0, timeUsed, precision: 2);
        }

        /// <summary>
        /// Verifies that SpeedUp costs less time (12s) than MaintainSpeed (15s).
        /// </summary>
        [Fact]
        public void ProcessTurn_SpeedUp_UsesLessTimeThanMaintain()
        {
            // Arrange
            var manager1 = CreateDefaultManager();
            manager1.StartRace();
            double time1Before = manager1.TimeRemaining;
            manager1.ProcessTurn(PlayerAction.SpeedUp);
            double speedUpTime = time1Before - manager1.TimeRemaining;

            var manager2 = CreateDefaultManager();
            manager2.StartRace();
            double time2Before = manager2.TimeRemaining;
            manager2.ProcessTurn(PlayerAction.MaintainSpeed);
            double maintainTime = time2Before - manager2.TimeRemaining;

            // Assert
            Assert.True(speedUpTime < maintainTime,
                $"SpeedUp time ({speedUpTime}s) should be less than Maintain time ({maintainTime}s)");
        }

        /// <summary>
        /// Verifies that the event log maintains a maximum of 5 entries.
        /// </summary>
        [Fact]
        public void EventLog_MaxFiveEntries()
        {
            // Arrange
            var manager = CreateDefaultManager();
            manager.StartRace(); // 1 event

            // Act — process 6 more turns for a total of 7 events
            for (int i = 0; i < 6; i++)
            {
                manager.ProcessTurn(PlayerAction.MaintainSpeed);
            }

            // Assert
            Assert.True(manager.EventLog.Count <= 5,
                $"Event log should have at most 5 entries but has {manager.EventLog.Count}");
        }

        /// <summary>
        /// Verifies that PitStop refuels the car by the expected amount.
        /// </summary>
        [Fact]
        public void ProcessTurn_PitStop_RefuelsCar()
        {
            // Arrange
            var car = new SportsCar();
            car.CurrentFuel = 20.0; // Low fuel (33% of 60L capacity)
            var track = new Track();
            var manager = new RaceManager(car, track);
            manager.StartRace();

            double fuelBefore = car.CurrentFuel;

            // Act
            manager.ProcessTurn(PlayerAction.PitStop);

            // Assert — PitStop refuels 20L, no fuel consumed
            Assert.Equal(fuelBefore + 20.0, car.CurrentFuel, precision: 2);
        }

        /// <summary>
        /// Verifies that the race state transitions to OutOfTime when time expires.
        /// </summary>
        [Fact]
        public void ProcessTurn_OutOfTime_SetsCorrectRaceState()
        {
            // Arrange — give only 10 seconds
            var car = new SportsCar();
            var track = new Track();
            var manager = new RaceManager(car, track, totalTimeSeconds: 10);
            manager.StartRace();

            // Act — SpeedUp costs 12 seconds, exceeding the 10s limit
            var result = manager.ProcessTurn(PlayerAction.SpeedUp);

            // Assert
            Assert.Equal(RaceState.OutOfTime, result.State);
        }
    }
}
