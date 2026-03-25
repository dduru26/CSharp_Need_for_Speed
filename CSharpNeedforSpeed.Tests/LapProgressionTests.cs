using Xunit;
using CSharpNeedforSpeed.Models;

namespace CSharpNeedforSpeed.Tests
{
    /// <summary>
    /// Unit tests for track and lap progression mechanics.
    /// Verifies that lap advancement, progress accumulation, and race completion
    /// work correctly.
    /// </summary>
    public class LapProgressionTests
    {
        /// <summary>
        /// Verifies that advancing the track by 1.0 (100% of a lap) completes
        /// the current lap and increments the lap counter.
        /// </summary>
        [Fact]
        public void Track_AdvanceLap_CompletesLapWhenProgressReaches100()
        {
            // Arrange
            var track = new Track();
            Assert.Equal(1, track.CurrentLap);

            // Act
            bool completed = track.AdvanceLap(1.0);

            // Assert
            Assert.True(completed, "Lap should be completed when progress reaches 100%");
            Assert.Equal(2, track.CurrentLap);
        }

        /// <summary>
        /// Verifies that adding 0.25 progress four times accumulates to complete
        /// a full lap.
        /// </summary>
        [Fact]
        public void Track_MultipleAdvances_AccumulatesProgress()
        {
            // Arrange
            var track = new Track();

            // Act
            bool lap1 = track.AdvanceLap(0.25);
            bool lap2 = track.AdvanceLap(0.25);
            bool lap3 = track.AdvanceLap(0.25);
            bool lap4 = track.AdvanceLap(0.25);

            // Assert
            Assert.False(lap1, "First advance should not complete a lap");
            Assert.False(lap2, "Second advance should not complete a lap");
            Assert.False(lap3, "Third advance should not complete a lap");
            Assert.True(lap4, "Fourth advance of 0.25 should complete the lap (total = 1.0)");
            Assert.Equal(2, track.CurrentLap);
        }

        /// <summary>
        /// Verifies that the race is marked as complete after all five laps are finished.
        /// </summary>
        [Fact]
        public void Track_IsRaceComplete_ReturnsTrueAfterFiveLaps()
        {
            // Arrange
            var track = new Track(totalLaps: 5);

            // Act — complete 5 full laps
            for (int i = 0; i < 5; i++)
            {
                track.AdvanceLap(1.0);
            }

            // Assert
            Assert.True(track.IsRaceComplete(),
                "Race should be complete after 5 laps");
            Assert.True(track.CurrentLap > track.TotalLaps,
                "CurrentLap should exceed TotalLaps when race is complete");
        }

        /// <summary>
        /// Verifies that the race is not complete when fewer than five laps are finished.
        /// </summary>
        [Fact]
        public void Track_IsRaceComplete_ReturnsFalseBeforeFiveLaps()
        {
            // Arrange
            var track = new Track(totalLaps: 5);

            // Act — complete only 3 laps
            for (int i = 0; i < 3; i++)
            {
                track.AdvanceLap(1.0);
            }

            // Assert
            Assert.False(track.IsRaceComplete(),
                "Race should not be complete after only 3 laps");
            Assert.Equal(4, track.CurrentLap);
        }

        /// <summary>
        /// Verifies that excess progress beyond 1.0 carries over to the next lap.
        /// </summary>
        [Fact]
        public void Track_AdvanceLap_CarriesOverExcessProgress()
        {
            // Arrange
            var track = new Track();

            // Act — advance by more than one full lap
            track.AdvanceLap(1.3);

            // Assert
            Assert.Equal(2, track.CurrentLap);
            Assert.Equal(0.3, track.LapProgress, precision: 2);
        }

        /// <summary>
        /// Verifies that the progress bar output contains the expected format
        /// and percentage display.
        /// </summary>
        [Fact]
        public void Track_GetProgressBar_ReturnsFormattedString()
        {
            // Arrange
            var track = new Track();
            track.LapProgress = 0.5;

            // Act
            string progressBar = track.GetProgressBar(20);

            // Assert
            Assert.Contains("[", progressBar);
            Assert.Contains("]", progressBar);
            Assert.Contains("50%", progressBar);
        }

        /// <summary>
        /// Verifies that a completed race shows 100% in the progress bar.
        /// </summary>
        [Fact]
        public void Track_GetProgressBar_Shows100WhenComplete()
        {
            // Arrange
            var track = new Track(totalLaps: 5);
            for (int i = 0; i < 5; i++)
            {
                track.AdvanceLap(1.0);
            }

            // Act
            string progressBar = track.GetProgressBar();

            // Assert
            Assert.Contains("100%", progressBar);
        }
    }
}
