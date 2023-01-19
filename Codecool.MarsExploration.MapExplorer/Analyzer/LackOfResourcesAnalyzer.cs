using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Analyzer;

public class LackOfResourcesAnalyzer : IOutcomeAnalyzer
{
    //If false the simulation should continue
    // If true - simulation stops with Error Outcome
    // - analyzer could check whether the rover has almost explored the whole chart and no right condition has been found.

    
    public ExplorationOutcome Analyze(SimulationContext simulationContext, int step)
    {
        var mapCharts = simulationContext.Map.Representation.GetLength(0) * simulationContext.Map.Representation.GetLength(1);
        var roverExplored = simulationContext.Rovers.First().EncounteredResources.Count();
        
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
        
        if ((double)roverExplored / mapCharts > 0.009 && minerals.Count() < 4 && waters.Count() < 3)
        {
            return ExplorationOutcome.Error;
        }

        return ExplorationOutcome.InProgress;
    }
}