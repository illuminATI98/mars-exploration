using Codecool.MarsExploration.MapExplorer.Analyzer;
using Codecool.MarsExploration.MapExplorer.Configuration.Service;
using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Movement;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapExplorer.UI;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service;

public class ExplorationSimulator : IExplorationSimulator
{
    private readonly IMapLoader _mapLoader;
    private  readonly Random Random = new Random();
    private  IConfigurationValidator _configurationValidator;
    private  IOutcomeAnalyzer _lackOfResourcesAnalyzer;
    private  IOutcomeAnalyzer _succesAnalyzer;
    private  IOutcomeAnalyzer _timeOutanalyzer;
    private IRoverDeployer _roverDeployer;
    private SimulationStepLoggingUi _simulationStepLoggingUi;
    

    public ExplorationSimulator(IMapLoader mapLoader, IConfigurationValidator configurationValidator, IOutcomeAnalyzer lackOfResourcesAnalyzer, IOutcomeAnalyzer succesAnalyzer, IOutcomeAnalyzer timeOutanalyzer, IRoverDeployer roverDeployer, SimulationStepLoggingUi simulationStepLoggingUi)
    {
        _mapLoader = mapLoader;
        _configurationValidator = configurationValidator;
        _lackOfResourcesAnalyzer = lackOfResourcesAnalyzer;
        _succesAnalyzer = succesAnalyzer;
        _timeOutanalyzer = timeOutanalyzer;
        _roverDeployer = roverDeployer;
        _simulationStepLoggingUi = simulationStepLoggingUi;
    }

    public void RunSimulation(Configuration.Configuration configuration)
    {
        var map = _mapLoader.Load(configuration.MapFile);
        var landingSpot = CheckLandingSpotForClear(configuration.LandingSpot, map);
        // var rover = _roverDeployer.Deploy();
        // var simulationContext = new SimulationContext(0, configuration.StepsToTimeOut, rover,
        //     landingSpot, map, configuration.SymbolsOfTheResources);
        // ExploringRoutine exploringRoutine = new ExploringRoutine(simulationContext);
        //
        // SimulationLoop(simulationContext, exploringRoutine);
    }

    public SimulationContext HandleOutcome(SimulationContext simulationContext, ExplorationOutcome outcome)
    {
        return simulationContext with { ExplorationOutcome = outcome };
    }

    private void SimulationLoop(SimulationContext simulationContext, ExploringRoutine exploringRoutine)
    {
        int step = 1;
        while (simulationContext.ExplorationOutcome == ExplorationOutcome.InProgress &&
               simulationContext.StepsToReachTimeOut >= step)
        {
            _simulationStepLoggingUi.Run(simulationContext, step);
            //exploringRoutine.Step(simulationContext.Rover);
            var results = new[]
            {
                _lackOfResourcesAnalyzer.Analyze(simulationContext, step),
                _succesAnalyzer.Analyze(simulationContext, step),
                _timeOutanalyzer.Analyze(simulationContext, step)
            };
            if (results.Any(s => s != ExplorationOutcome.InProgress))
            {
                var outcome = results.Single(s => s != ExplorationOutcome.InProgress);
                simulationContext = HandleOutcome(simulationContext, outcome);
                _simulationStepLoggingUi.Run(simulationContext, step);
            }

            step++;
        }
    }
    private Coordinate CheckLandingSpotForClear(Coordinate landingCoordinate, Map map)
    {
        while (!_configurationValidator.LandingSpotValidate(map,landingCoordinate))
        {
            landingCoordinate = new Coordinate(Random.Next(map.Representation.GetLength(0)),
                Random.Next(map.Representation.GetLength(0)));
        }

        return landingCoordinate;
    }
}