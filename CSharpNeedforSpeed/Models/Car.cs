namespace CSharpNeedforSpeed.Models
{
    /// <summary>
    /// Abstract base class representing a car in the C# Speed Rush racing game.
    /// Each car has unique speed, fuel consumption, and capacity characteristics
    /// that affect gameplay strategy.
    /// </summary>
    public abstract class Car
    {
        /// <summary>
        /// Gets the display name of the car.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the maximum speed of the car in kilometres per hour.
        /// </summary>
        public double MaxSpeed { get; protected set; }

        /// <summary>
        /// Gets the base fuel consumption rate per turn at maximum speed, in litres.
        /// </summary>
        public double FuelConsumptionRate { get; protected set; }

        /// <summary>
        /// Gets the total fuel tank capacity in litres.
        /// </summary>
        public double FuelCapacity { get; protected set; }

        /// <summary>
        /// Gets or sets the current fuel level in the tank, in litres.
        /// </summary>
        public double CurrentFuel { get; set; }

        /// <summary>
        /// Gets or sets the current speed of the car in kilometres per hour.
        /// </summary>
        public double CurrentSpeed { get; set; }

        /// <summary>
        /// Gets the amount of fuel refuelled during a pit stop, in litres.
        /// </summary>
        public abstract double PitStopRefuelAmount { get; }

        /// <summary>
        /// Gets the time penalty in seconds incurred during a pit stop.
        /// </summary>
        public abstract double PitStopTimePenalty { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Car"/> class with the specified parameters.
        /// </summary>
        /// <param name="name">The display name of the car.</param>
        /// <param name="maxSpeed">The maximum speed in km/h.</param>
        /// <param name="fuelConsumptionRate">The base fuel consumption rate per turn.</param>
        /// <param name="fuelCapacity">The total fuel tank capacity in litres.</param>
        protected Car(string name, double maxSpeed, double fuelConsumptionRate, double fuelCapacity)
        {
            Name = name;
            MaxSpeed = maxSpeed;
            FuelConsumptionRate = fuelConsumptionRate;
            FuelCapacity = fuelCapacity;
            CurrentFuel = fuelCapacity;
            CurrentSpeed = 0;
        }

        /// <summary>
        /// Calculates the fuel usage for a given player action. Each car subclass
        /// implements its own fuel consumption logic with unique multipliers.
        /// </summary>
        /// <param name="action">The player action to calculate fuel usage for.</param>
        /// <returns>The amount of fuel consumed in litres for the given action.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unrecognized player action is provided.</exception>
        public abstract double CalculateFuelUsage(PlayerAction action);

        /// <summary>
        /// Calculates the distance gained as a fraction of a lap (0.0 to 1.0)
        /// for a given player action.
        /// </summary>
        /// <param name="action">The player action to calculate distance for.</param>
        /// <returns>A value between 0.0 and 1.0 representing the fraction of a lap completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unrecognized player action is provided.</exception>
        public virtual double CalculateDistanceGained(PlayerAction action)
        {
            return action switch
            {
                PlayerAction.SpeedUp => GetSpeedUpDistance(),
                PlayerAction.MaintainSpeed => GetMaintainDistance(),
                PlayerAction.PitStop => 0.0,
                _ => throw new ArgumentOutOfRangeException(nameof(action), action, "Unrecognized player action.")
            };
        }

        /// <summary>
        /// Gets the distance fraction gained when speeding up. Override in subclasses for car-specific values.
        /// </summary>
        /// <returns>Distance as a fraction of a lap.</returns>
        protected abstract double GetSpeedUpDistance();

        /// <summary>
        /// Gets the distance fraction gained when maintaining speed. Override in subclasses for car-specific values.
        /// </summary>
        /// <returns>Distance as a fraction of a lap.</returns>
        protected abstract double GetMaintainDistance();

        /// <summary>
        /// Returns a formatted string representation of the car, including its name,
        /// max speed, fuel consumption rate, and fuel capacity.
        /// </summary>
        /// <returns>A human-readable string describing the car's attributes.</returns>
        public override string ToString()
        {
            return $"{Name} | Max Speed: {MaxSpeed} km/h | Fuel Rate: {FuelConsumptionRate}L/turn | Tank: {FuelCapacity}L";
        }
    }

    /// <summary>
    /// A high-performance sports car with excellent speed but high fuel consumption.
    /// Ideal for aggressive racing strategies where speed is prioritised over fuel economy.
    /// </summary>
    public class SportsCar : Car
    {
        /// <summary>
        /// Gets the amount of fuel refuelled during a pit stop (20 litres).
        /// </summary>
        public override double PitStopRefuelAmount => 20.0;

        /// <summary>
        /// Gets the time penalty incurred during a pit stop (10 seconds).
        /// </summary>
        public override double PitStopTimePenalty => 10.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SportsCar"/> class
        /// with preset performance characteristics.
        /// </summary>
        public SportsCar()
            : base("Sports Car", 220, 4.5, 60)
        {
        }

        /// <summary>
        /// Calculates fuel usage for the Sports Car based on the player action.
        /// SpeedUp uses 1.5x the base rate, MaintainSpeed uses 1.0x, and PitStop uses no fuel.
        /// </summary>
        /// <param name="action">The player action to calculate fuel for.</param>
        /// <returns>The fuel consumed in litres.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unrecognized action is provided.</exception>
        public override double CalculateFuelUsage(PlayerAction action)
        {
            return action switch
            {
                PlayerAction.SpeedUp => FuelConsumptionRate * 1.5,
                PlayerAction.MaintainSpeed => FuelConsumptionRate * 1.0,
                PlayerAction.PitStop => 0.0,
                _ => throw new ArgumentOutOfRangeException(nameof(action), action, "Unrecognized player action.")
            };
        }

        /// <summary>
        /// Gets the distance fraction gained when speeding up (0.25 of a lap).
        /// </summary>
        /// <returns>0.25 representing 25% of a lap.</returns>
        protected override double GetSpeedUpDistance() => 0.25;

        /// <summary>
        /// Gets the distance fraction gained when maintaining speed (0.18 of a lap).
        /// </summary>
        /// <returns>0.18 representing 18% of a lap.</returns>
        protected override double GetMaintainDistance() => 0.18;
    }

    /// <summary>
    /// An eco-friendly car with moderate speed and excellent fuel efficiency.
    /// Best suited for conservative strategies focused on completing the race
    /// without running out of fuel.
    /// </summary>
    public class EcoCar : Car
    {
        /// <summary>
        /// Gets the amount of fuel refuelled during a pit stop (15 litres).
        /// </summary>
        public override double PitStopRefuelAmount => 15.0;

        /// <summary>
        /// Gets the time penalty incurred during a pit stop (8 seconds).
        /// </summary>
        public override double PitStopTimePenalty => 8.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="EcoCar"/> class
        /// with preset eco-friendly performance characteristics.
        /// </summary>
        public EcoCar()
            : base("Eco Car", 160, 2.0, 45)
        {
        }

        /// <summary>
        /// Calculates fuel usage for the Eco Car based on the player action.
        /// SpeedUp uses 1.4x the base rate, MaintainSpeed uses 0.8x, and PitStop uses no fuel.
        /// </summary>
        /// <param name="action">The player action to calculate fuel for.</param>
        /// <returns>The fuel consumed in litres.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unrecognized action is provided.</exception>
        public override double CalculateFuelUsage(PlayerAction action)
        {
            return action switch
            {
                PlayerAction.SpeedUp => FuelConsumptionRate * 1.4,
                PlayerAction.MaintainSpeed => FuelConsumptionRate * 0.8,
                PlayerAction.PitStop => 0.0,
                _ => throw new ArgumentOutOfRangeException(nameof(action), action, "Unrecognized player action.")
            };
        }

        /// <summary>
        /// Gets the distance fraction gained when speeding up (0.18 of a lap).
        /// </summary>
        /// <returns>0.18 representing 18% of a lap.</returns>
        protected override double GetSpeedUpDistance() => 0.18;

        /// <summary>
        /// Gets the distance fraction gained when maintaining speed (0.15 of a lap).
        /// </summary>
        /// <returns>0.15 representing 15% of a lap.</returns>
        protected override double GetMaintainDistance() => 0.15;
    }

    /// <summary>
    /// A powerful muscle car with high speed and a large fuel tank, but very high fuel consumption.
    /// Offers the most distance per turn when speeding up, but burns through fuel quickly.
    /// </summary>
    public class MuscleCar : Car
    {
        /// <summary>
        /// Gets the amount of fuel refuelled during a pit stop (25 litres).
        /// </summary>
        public override double PitStopRefuelAmount => 25.0;

        /// <summary>
        /// Gets the time penalty incurred during a pit stop (12 seconds).
        /// </summary>
        public override double PitStopTimePenalty => 12.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MuscleCar"/> class
        /// with preset high-power performance characteristics.
        /// </summary>
        public MuscleCar()
            : base("Muscle Car", 200, 6.0, 70)
        {
        }

        /// <summary>
        /// Calculates fuel usage for the Muscle Car based on the player action.
        /// SpeedUp uses 1.6x the base rate, MaintainSpeed uses 1.1x, and PitStop uses no fuel.
        /// </summary>
        /// <param name="action">The player action to calculate fuel for.</param>
        /// <returns>The fuel consumed in litres.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unrecognized action is provided.</exception>
        public override double CalculateFuelUsage(PlayerAction action)
        {
            return action switch
            {
                PlayerAction.SpeedUp => FuelConsumptionRate * 1.6,
                PlayerAction.MaintainSpeed => FuelConsumptionRate * 1.1,
                PlayerAction.PitStop => 0.0,
                _ => throw new ArgumentOutOfRangeException(nameof(action), action, "Unrecognized player action.")
            };
        }

        /// <summary>
        /// Gets the distance fraction gained when speeding up (0.30 of a lap).
        /// </summary>
        /// <returns>0.30 representing 30% of a lap.</returns>
        protected override double GetSpeedUpDistance() => 0.30;

        /// <summary>
        /// Gets the distance fraction gained when maintaining speed (0.20 of a lap).
        /// </summary>
        /// <returns>0.20 representing 20% of a lap.</returns>
        protected override double GetMaintainDistance() => 0.20;
    }
}
