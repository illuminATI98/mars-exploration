using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Logger;

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
    public void Position(Rover rover,SimulationContext simulationContext)
    {
        Console.WriteLine($"STEP {simulationContext.Step}; EVENT position; UNIT {rover.Id};POSITION [{rover.CurrentPosition.X},{rover.CurrentPosition.Y}]");
    }

    public void TargetPosition(Rover rover, SimulationContext simulationContext, Coordinate coordinate)
    {
        Console.WriteLine($"STEP {simulationContext.Step}; EVENT target_position; UNIT {rover.Id}; TARGET POSITION [{coordinate.X},{coordinate.Y}]\n");
    }
    public void Extracting(Rover rover, SimulationContext simulationContext)
    {
        Console.WriteLine($"\nSTEP {simulationContext.Step}; EVENT extraction; UNIT {rover.Id}; RESOURCE {rover.Resource}; PROGRESS {rover.Progress}/{rover.ResourcesToMine}\n");
    }

    public void Delivering(Rover rover, SimulationContext simulationContext)
    {
        Console.WriteLine($"\nSTEP {simulationContext.Step}; EVENT delivery; UNIT {rover.Id}; RESOURCE {rover.Resource}; TO {simulationContext.CommandCenters.First().Id}\n");
    }

    public void Position(CommandCenter.Model.CommandCenter commandCenter,SimulationContext simulationContext)
    {
        Console.WriteLine($"STEP {simulationContext.Step}; EVENT position; UNIT {commandCenter.Id};POSITION [{commandCenter.CurrentPosition.X},{commandCenter.CurrentPosition.Y}]");
    }
    public void OutCome(SimulationContext simulationContext)
    {
        Console.WriteLine($"\nSTEP {simulationContext.Step}; EVENT outcome; OUTCOME {simulationContext.ExplorationOutcome}\n");
    }
    public void Construction(SimulationContext simulationContext)
    {
        Console.WriteLine($"STEP {simulationContext.Step}; EVENT construction; UNIT {simulationContext.Construction.UnitId}; POSITION [{simulationContext.Construction.Position.X},{simulationContext.Construction.Position.Y}]; PROGRESS {simulationContext.Construction.Progress} of {simulationContext.Construction.StepsToComplete}");
    }

    public void Final(SimulationContext simulationContext)
    {
        Console.Write($"STEP {simulationContext.Step}; EVENT simulation over; BUILT COMMAND CENTRES {simulationContext.CommandCenters.First().Id}; " +
                          $"ROVERS  ");
        foreach (var rover in simulationContext.Rovers)
        {
             Console.Write($"{rover.Id} ");
        }
        
        Console.Write($"MINERALS: {simulationContext.CommandCenters.First().MineralResources}; ");
        Console.Write($"WATERS: {simulationContext.CommandCenters.First().WaterResources};");
    }
}