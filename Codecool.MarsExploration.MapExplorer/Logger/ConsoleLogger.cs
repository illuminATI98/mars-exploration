using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Logger;

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
    public void Position(Rover rover,SimulationContext simulationContext)
    {
        if (rover.CurrentRoutine == Routine.Extracting)
        {
            Console.WriteLine($"STEP {simulationContext.Step}; EVENT extraction; UNIT {rover.Id}; RESOURCE {rover.Resource}; PROGRESS {rover.Progress}");
        }
        else if (rover.CurrentRoutine == Routine.Delivering)
        {
            Console.WriteLine($"STEP {simulationContext.Step}; EVENT delivery; UNIT {rover.Id}; RESOURCE {rover.Resource}; PROGRESS {rover.Progress}");
        }
        else
        {
            Console.WriteLine($"STEP {simulationContext.Step}; EVENT position; UNIT {rover.Id};POSITION [{rover.CurrentPosition.X},{rover.CurrentPosition.Y}]");
        }
    }
    public void Position(CommandCenter.Model.CommandCenter commandCenter,SimulationContext simulationContext)
    {
        Console.WriteLine($"STEP {simulationContext.Step}; EVENT position; UNIT {commandCenter.Id};POSITION [{commandCenter.CurrentPosition.X},{commandCenter.CurrentPosition.Y}]");
    }
    public void OutCome(SimulationContext simulationContext)
    {
        Console.WriteLine($"STEP {simulationContext.Step}; EVENT outcome; OUTCOME {simulationContext.ExplorationOutcome}");
    }
    public void Construction(SimulationContext simulationContext)
    {
        Console.WriteLine($"STEP {simulationContext.Step}; EVENT construction; UNIT {simulationContext.Construction.UnitId}; POSITION [{simulationContext.Construction.Position.X},{simulationContext.Construction.Position.Y}]; PROGRESS {simulationContext.Construction.Progress} of {simulationContext.Construction.StepsToComplete}");
    }
}