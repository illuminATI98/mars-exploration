using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Codecool.MarsExploration.MapExplorer.CommandCenter.Model;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Task = Codecool.MarsExploration.MapExplorer.MarsRover.Task;

namespace Codecool.MarsExploration.MapExplorer.Construction.Service;

public class Builder : IBuilder
{
    private readonly ILogger _logger;
    
    public Builder(ILogger logger)
    {
        _logger = logger;
    }
    public SimulationContext Build(SimulationContext simulationContext, string unit)
    {
        if (unit == "center")
        {
            var center = simulationContext.CommandCenters.Last();
            var construction = new Model.Construction(center.Id, center.CurrentPosition, 0, 10);
            
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
        }

        if (unit == "rover")
        {
            var center = simulationContext.CommandCenters.Last();
            var roverIds = new List<string>();
            foreach (var rover in simulationContext.Rovers)
            {
                roverIds.Add(rover.Id);
            }
            var lastId = roverIds.Last();
            var newId = Regex.Replace(lastId, @"\d+$", match => (int.Parse(match.Value) + 1).ToString());
            
            var construction = new Model.Construction(newId, center.CurrentPosition, 0, 10);
            
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

            var newRover = new Rover(construction.UnitId, construction.Position, null, null, Routine.Exploring, null, 0, Task.WaterGathering,0);
            var rovers = simulationContext.Rovers.ToList();
            rovers.Add(newRover);
            simulationContext = simulationContext with
            {
                Rovers = rovers
            };
        }
        return simulationContext;
    }
}