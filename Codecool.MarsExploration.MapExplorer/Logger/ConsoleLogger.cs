using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Logger;

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }

    public void Position(Rover rover)
    {
        Console.Write($"EVENT position; UNIT {rover.Id};POSITION [{rover.CurrentPosition.X},{rover.CurrentPosition.Y}]");
    }

    public void Position(CommandCenter.Model.CommandCenter commandCenter)
    {
        Console.Write($"EVENT position; UNIT {commandCenter.Id};POSITION [{commandCenter.CurrentPosition.X},{commandCenter.CurrentPosition.Y}]");
    }

    public void Step(SimulationContext simulationContext)
    {
        Console.WriteLine($"STEP {simulationContext.Step};");
    }

    public void OutCome(SimulationContext simulationContext)
    {
        Console.Write($"EVENT outcome; OUTCOME {simulationContext.ExplorationOutcome}");
    }

    public void Construction(Construction.Model.Construction construction)
    {
        Console.Write($"EVENT construction; UNIT {construction.UnitId}; POSITION [{construction.Position.X},{construction.Position.Y}]; PROGRESS {construction.Progress} of {construction.StepsToComplete}");
    }
}