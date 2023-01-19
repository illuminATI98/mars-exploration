using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Codecool.MarsExploration.MapExplorer.CommandCenter.Model;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Construction.Service;

public class Builder : IBuilder
{
    private ILogger _logger;
    
    public Builder(ILogger logger)
    {
        _logger = logger;
    }
    public SimulationContext Build(SimulationContext simulationContext, Model.Construction construction)
    {
        simulationContext = simulationContext with
        {
            Construction = construction
        };
        while (simulationContext.Construction.Progress < simulationContext.Construction.StepsToComplete)
        {
            simulationContext.Construction.Progress++;
            
            simulationContext = simulationContext with
            {
                Step = simulationContext.Step + 1
            };
            _logger.Construction(simulationContext);
        }
        foreach (var commandCenter in simulationContext.CommandCenters)
        {
            if (commandCenter.Id == construction.UnitId)
            {
                commandCenter.Status = Status.Active;
                return simulationContext;
            }
        }

        // var roverIds = new List<string>();
        // foreach (var rover in simulationContext.Rovers)
        // {
        //     roverIds.Add(rover.Id);
        // }
        //
        // if (!roverIds.Contains(construction.UnitId))
        // {
        //     var lastId = roverIds.Last();
        //     var newId = Regex.Replace(lastId, @"\d+$", match => (int.Parse(match.Value) + 1).ToString());
        //     
        // }
        
        return simulationContext;
    }
}