using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.UI;

public class SimulationStepLoggingUi
{
    private readonly ILogger _logger;

    public SimulationStepLoggingUi(ILogger logger)
    {
        _logger = logger;
    }

    public void Run(SimulationContext simulationContext)
    {
        
        switch (simulationContext.ExplorationOutcome)
        {
            case ExplorationOutcome.InProgress:
                foreach (var rover in simulationContext.Rovers)
                {
                    _logger.Step(simulationContext);
                    _logger.Position(rover);
                }
                foreach (var construction in simulationContext.Constructions)
                {
                    _logger.Step(simulationContext);
                    _logger.Construction(construction);
                }
                break;
            case ExplorationOutcome.Colonizable:
                _logger.Step(simulationContext);
                _logger.OutCome(simulationContext);
                break;
            case ExplorationOutcome.Error:
                _logger.Step(simulationContext);
                _logger.OutCome(simulationContext);
                break;
            case ExplorationOutcome.Timeout:
                _logger.Step(simulationContext);
                _logger.OutCome(simulationContext);
                break;
        }
        
    }
}