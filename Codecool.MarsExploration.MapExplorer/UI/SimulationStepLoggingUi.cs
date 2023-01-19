using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MarsRover;
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
            case ExplorationOutcome.Colonizable:
                
                _logger.OutCome(simulationContext);
                break;
            case ExplorationOutcome.Error:
                
                _logger.OutCome(simulationContext);
                break;
            case ExplorationOutcome.Timeout:
                
                _logger.OutCome(simulationContext);
                break;
        }

        foreach (var simulationContextRover in simulationContext.Rovers)
        {
            switch (simulationContextRover.CurrentRoutine)
            {
                case Routine.Exploring:
                    _logger.Position(simulationContextRover, simulationContext);
                    break;
                case Routine.Extracting:
                    _logger.Extracting(simulationContextRover, simulationContext);
                        break;
            }
        }
    }
}