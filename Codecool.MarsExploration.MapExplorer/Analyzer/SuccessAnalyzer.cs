using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Analyzer;

public class SuccessAnalyzer : IOutcomeAnalyzer
{
    //If false the simulation should continue
    //If true - It was found that there are 4 minerals and 3 waters found in total - Should stop the simulation with colonizable Outcome
    
    public ExplorationOutcome Analyze(SimulationContext simulationContext, int step)
    {
        var encounteredCoordinates = simulationContext.Rovers.First().EncounteredResources;
        var map = simulationContext.Map.Representation;
        var minerals = new List<string?>();
        var waters = new List<string?>();

        foreach (var coordinate in encounteredCoordinates)
        {
            if (map[coordinate.X, coordinate.Y] == "%")
            {
                minerals.Add(map[coordinate.X, coordinate.Y]);
            }
            else if (map[coordinate.X, coordinate.Y] == "*")
            {
                waters.Add(map[coordinate.X, coordinate.Y]);
            }
        }

        if (minerals.Count() >= 4 && waters.Count() >= 3)
        {
            return ExplorationOutcome.Colonizable;
        }

        return ExplorationOutcome.InProgress;
    }
}