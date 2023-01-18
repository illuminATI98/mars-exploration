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

    public void Run(SimulationContext simulationContext, int step)
    {
        switch (simulationContext.ExplorationOutcome)
        {
            case ExplorationOutcome.InProgress:
                _logger.Log($"STEP {step} EVENT position UNIT {simulationContext.Rovers.First().Id} POSITION [{simulationContext.Rovers.First().CurrentPosition.X}, {simulationContext.Rovers.First().CurrentPosition.Y}]");
                break;
            case ExplorationOutcome.Colonizable:
                _logger.Log(
                    $"STEP {step} EVENT outcome OUTCOME {simulationContext.ExplorationOutcome}");
                break;
            case ExplorationOutcome.Error:
                _logger.Log($"STEP {step} EVENT outcome OUTCOME {simulationContext.ExplorationOutcome}");
                break;
            case ExplorationOutcome.Timeout:
                _logger.Log($"STEP {step} EVENT outcome OUTCOME {simulationContext.ExplorationOutcome}");
                break;
        }
        
    }
}