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
    private IGetLocationOfCommanCentre _getLocationOfCommandCentre;
    

    public ExplorationSimulator(IMapLoader mapLoader, IConfigurationValidator configurationValidator, IOutcomeAnalyzer lackOfResourcesAnalyzer, 
        IOutcomeAnalyzer succesAnalyzer, IOutcomeAnalyzer timeOutanalyzer, IRoverDeployer roverDeployer, 
        SimulationStepLoggingUi simulationStepLoggingUi, IGetLocationOfCommanCentre getLocationOfCommandCentre)
    {
        _mapLoader = mapLoader;
        _configurationValidator = configurationValidator;
        _lackOfResourcesAnalyzer = lackOfResourcesAnalyzer;
        _succesAnalyzer = succesAnalyzer;
        _timeOutanalyzer = timeOutanalyzer;
        _roverDeployer = roverDeployer;
        _simulationStepLoggingUi = simulationStepLoggingUi;
        _getLocationOfCommandCentre = getLocationOfCommandCentre;
    }

    public void RunSimulation(Configuration.Configuration configuration)
    {
        var map = _mapLoader.Load(configuration.MapFile);
        
        //Landing spot for the spaceship
        var landingSpot = CheckLandingSpotForClear(configuration.LandingSpot, map);
        
        //Rover deployer in an adjacent coordinate
        var rover1 = _roverDeployer.Deploy();
        
        //Rover one begins its exploration routine
        var simulationContext = new SimulationContext(0, configuration.StepsToTimeOut, new List<Rover>{rover1},null,
            landingSpot, map, configuration.SymbolsOfTheResources);
        ExploringRoutine exploringRoutine = new ExploringRoutine(simulationContext);
        
        //Rover one finds a colonisable spot
        while (simulationContext.ExplorationOutcome != ExplorationOutcome.Colonizable)
        {
            simulationContext = SimulationLoop(new SimulationContext(0, configuration.StepsToTimeOut, new List<Rover>{rover1},null,
                landingSpot, map, configuration.SymbolsOfTheResources), exploringRoutine);
        }

        var colonizableSpot = simulationContext.Rovers.First().CurrentPosition;
        
        //Choose the location of the future command centre
        var locationOfTheCommandCentre = _getLocationOfCommandCentre.GetCentreLocation(colonizableSpot, simulationContext);
        Console.WriteLine(locationOfTheCommandCentre);
        
        //Rover one extracts minerals and gathers them at the chosen command centre Coordinate


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

    public SimulationContext HandleOutcome(SimulationContext simulationContext, ExplorationOutcome outcome)
    {
        return simulationContext with { ExplorationOutcome = outcome };
    }

    private SimulationContext SimulationLoop(SimulationContext simulationContext, ExploringRoutine exploringRoutine)
    {
        int step = 1;
        while (simulationContext.ExplorationOutcome == ExplorationOutcome.InProgress &&
               simulationContext.StepsToReachTimeOut >= step)
        {
            _simulationStepLoggingUi.Run(simulationContext, step);
            exploringRoutine.Step(simulationContext.Rovers.First());
            var results = new[]
            {
                _lackOfResourcesAnalyzer.Analyze(simulationContext, step),
                _succesAnalyzer.Analyze(simulationContext, step),
                _timeOutanalyzer.Analyze(simulationContext, step)
            };
            if (results.Any(s => s != ExplorationOutcome.InProgress))
            {
                var outcome = results.First(s => s != ExplorationOutcome.InProgress);
                simulationContext = HandleOutcome(simulationContext, outcome);
                _simulationStepLoggingUi.Run(simulationContext, step);
            }

            step++;
        }

        return simulationContext;
    }
}