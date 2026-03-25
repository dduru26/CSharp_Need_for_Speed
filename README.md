# C# Need For Speed

A turn-based, time-focused racing simulation game built with Avalonia UI and .NET.

## Game Description

C# Need For Speed puts you behind the wheel in a strategic turn-based racing game. Select from three unique cars — each with their own speed, fuel efficiency, and pit stop characteristics — then race across 5 laps on the Need For Speed Circuit. Every turn, you must decide: push for speed, maintain your pace, or make a pit stop to refuel. Balance aggression with fuel management to cross the finish line before time runs out.

## Features

- **3 Unique Cars**: Sports Car, Eco Car, and Muscle Car — each with distinct stats and trade-offs
- **Turn-Based Strategy**: Choose from Speed Up, Maintain Speed, or Pit Stop each turn
- **Real-Time Countdown**: A ticking timer adds urgency to your strategic decisions
- **Fuel Management**: Monitor your fuel gauge and plan pit stops carefully
- **Lap Progression**: Track your progress with visual progress bars and lap counters
- **Event Log**: Real-time console-style log of your last 5 race events
- **Race Summary**: End-of-race results screen with full statistics
- **Dark Theme UI**: Sleek, modern dark interface built with Avalonia Fluent theme
- **Full Unit Test Coverage**: Comprehensive xUnit tests for game logic

## Cars

| Car | Max Speed | Fuel Rate | Tank | SpeedUp Gain | Maintain Gain | Pit Refuel | Pit Penalty |
|-----|-----------|-----------|------|--------------|---------------|------------|-------------|
| Sports Car | 220 km/h | 4.5 L/turn | 60 L | 25% | 18% | 20 L | 10s |
| Eco Car | 160 km/h | 2.0 L/turn | 45 L | 18% | 15% | 15 L | 8s |
| Muscle Car | 200 km/h | 6.0 L/turn | 70 L | 30% | 20% | 25 L | 12s |

## How to Play

1. **Launch the game** and click "Start Race" on the main menu
2. **Select your car** from the car selection screen — review each car's stats on the right panel
3. **Click "Start Race!"** to begin the race
4. **Each turn**, choose one of three actions:
   - **Speed Up**: Gain more distance per turn but burn extra fuel (12s per turn)
   - **Maintain Speed**: Steady progress with balanced fuel usage (15s per turn)
   - **Pit Stop**: Refuel your car — no distance gained, time penalty applies (15s + car-specific penalty)
5. **Monitor your gauges**: Watch the fuel bar (green) and time bar (blue) carefully
6. **Complete 5 laps** before running out of fuel or time to win!

### Win/Lose Conditions

- **Victory**: Complete all 5 laps before time or fuel runs out
- **Game Over (Fuel)**: Your car runs out of fuel mid-race
- **Game Over (Time)**: The 300-second timer expires

## Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) or later

## How to Run

```bash
# Clone or navigate to the project directory
cd CSharp_Need_for_Speed

# Restore packages
dotnet restore

# Run the application
dotnet run --project CSharpNeedforSpeed
```

## How to Test

```bash
# Run all unit tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run a specific test class
dotnet test --filter "FullyQualifiedName~FuelTests"
```

## Project Structure

```
CSharp_Need_for_Speed/
├── CSharpNeedforSpeed.sln
├── README.md
├── CSharpNeedforSpeed/
│   ├── CSharpNeedforSpeed.csproj
│   ├── Program.cs
│   ├── App.axaml
│   ├── App.axaml.cs
│   ├── app.manifest
│   ├── Models/
│   │   ├── Enums.cs          # PlayerAction, RaceState, LapRecord
│   │   ├── Car.cs            # Abstract Car + SportsCar, EcoCar, MuscleCar
│   │   ├── Track.cs          # Lap progression and progress tracking
│   │   ├── RaceResult.cs     # Turn result data structure
│   │   └── RaceManager.cs    # Core game loop and state management
│   ├── ViewModels/
│   │   ├── MainWindowViewModel.cs       # Main menu logic
│   │   ├── CarSelectionViewModel.cs     # Car selection and stats display
│   │   └── RaceWindowViewModel.cs       # Race UI state management
│   └── Views/
│       ├── MainWindow.axaml + .cs       # Main menu UI
│       ├── CarSelectionWindow.axaml + .cs # Car picker UI
│       └── RaceWindow.axaml + .cs       # Race gameplay UI
└── CSharpNeedforSpeed.Tests/
    ├── CSharpNeedforSpeed.Tests.csproj
    ├── FuelTests.cs              # Fuel consumption tests
    ├── LapProgressionTests.cs    # Track/lap logic tests
    └── RaceManagerTests.cs       # Game loop and state tests
```

## Architecture

- **MVVM Pattern**: Views bind to ViewModels using CommunityToolkit.Mvvm
- **Avalonia UI**: Cross-platform desktop UI framework with Fluent theme
- **Turn-Based Game Loop**: `RaceManager.ProcessTurn()` is the core method
- **Event-Driven Navigation**: ViewModels raise events; code-behind handles window management
