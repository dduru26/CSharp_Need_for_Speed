using Xunit;
using CSharpNeedforSpeed.Models;

namespace CSharpNeedforSpeed.Tests
{
    /// <summary>
    /// Unit tests for fuel consumption mechanics across different car types.
    /// Verifies that each car consumes fuel at the correct rate for each action
    /// and that fuel-related exceptions are thrown appropriately.
    /// </summary>
    public class FuelTests
    {
        /// <summary>
        /// Verifies that the Sports Car reduces fuel by the correct amount (4.5 * 1.5 = 6.75L)
        /// when the SpeedUp action is taken.
        /// </summary>
        [Fact]
        public void SportsCar_SpeedUp_ReducesFuelCorrectly()
        {
            // Arrange
            var car = new SportsCar();
            double initialFuel = car.CurrentFuel;
            double expectedFuelUsage = 4.5 * 1.5; // 6.75

            // Act
            double fuelUsed = car.CalculateFuelUsage(PlayerAction.SpeedUp);

            // Assert
            Assert.Equal(expectedFuelUsage, fuelUsed, precision: 2);

            // Also verify that applying this to the car state works correctly
            car.CurrentFuel -= fuelUsed;
            Assert.Equal(initialFuel - expectedFuelUsage, car.CurrentFuel, precision: 2);
        }

        /// <summary>
        /// Verifies that the Eco Car consumes less fuel than the Sports Car
        /// when both perform the same MaintainSpeed action.
        /// </summary>
        [Fact]
        public void EcoCar_ConsumesFuelSlowerThanSportsCar()
        {
            // Arrange
            var ecoCar = new EcoCar();
            var sportsCar = new SportsCar();

            // Act
            double ecoFuelUsed = ecoCar.CalculateFuelUsage(PlayerAction.MaintainSpeed);
            double sportsFuelUsed = sportsCar.CalculateFuelUsage(PlayerAction.MaintainSpeed);

            // Assert
            Assert.True(ecoFuelUsed < sportsFuelUsed,
                $"Eco car fuel usage ({ecoFuelUsed}L) should be less than sports car fuel usage ({sportsFuelUsed}L)");
        }

        /// <summary>
        /// Verifies that attempting a pit stop when the fuel tank is at 95% capacity
        /// throws an <see cref="InvalidOperationException"/> with the correct message.
        /// </summary>
        [Fact]
        public void PitStop_OverfullFuel_ThrowsException()
        {
            // Arrange
            var car = new SportsCar();
            car.CurrentFuel = car.FuelCapacity * 0.95; // 95% full
            var track = new Track();
            var manager = new RaceManager(car, track);
            manager.StartRace();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                manager.ProcessTurn(PlayerAction.PitStop));
            Assert.Contains("90%", exception.Message);
        }

        /// <summary>
        /// Verifies that the Muscle Car has the highest fuel consumption rate
        /// among all car types when speeding up.
        /// </summary>
        [Fact]
        public void MuscleCar_HasHighestFuelConsumption()
        {
            // Arrange
            var sportsCar = new SportsCar();
            var ecoCar = new EcoCar();
            var muscleCar = new MuscleCar();

            // Act
            double sportsFuel = sportsCar.CalculateFuelUsage(PlayerAction.SpeedUp);
            double ecoFuel = ecoCar.CalculateFuelUsage(PlayerAction.SpeedUp);
            double muscleFuel = muscleCar.CalculateFuelUsage(PlayerAction.SpeedUp);

            // Assert
            Assert.True(muscleFuel > sportsFuel,
                $"Muscle car fuel ({muscleFuel}L) should be > sports car fuel ({sportsFuel}L)");
            Assert.True(muscleFuel > ecoFuel,
                $"Muscle car fuel ({muscleFuel}L) should be > eco car fuel ({ecoFuel}L)");
        }

        /// <summary>
        /// Verifies that a pit stop action consumes zero fuel for all car types.
        /// </summary>
        [Fact]
        public void PitStop_ConsumesZeroFuel()
        {
            // Arrange
            var sportsCar = new SportsCar();
            var ecoCar = new EcoCar();
            var muscleCar = new MuscleCar();

            // Act & Assert
            Assert.Equal(0.0, sportsCar.CalculateFuelUsage(PlayerAction.PitStop));
            Assert.Equal(0.0, ecoCar.CalculateFuelUsage(PlayerAction.PitStop));
            Assert.Equal(0.0, muscleCar.CalculateFuelUsage(PlayerAction.PitStop));
        }

        /// <summary>
        /// Verifies that all cars start with a full fuel tank equal to their capacity.
        /// </summary>
        [Fact]
        public void AllCars_StartWithFullFuel()
        {
            // Arrange & Act
            var sportsCar = new SportsCar();
            var ecoCar = new EcoCar();
            var muscleCar = new MuscleCar();

            // Assert
            Assert.Equal(sportsCar.FuelCapacity, sportsCar.CurrentFuel);
            Assert.Equal(ecoCar.FuelCapacity, ecoCar.CurrentFuel);
            Assert.Equal(muscleCar.FuelCapacity, muscleCar.CurrentFuel);
        }
    }
}
