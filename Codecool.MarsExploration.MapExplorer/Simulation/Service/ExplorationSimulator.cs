using Codecool.MarsExploration.MapExplorer.Analyzer;
using Codecool.MarsExploration.MapExplorer.Configuration.Service;
using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Movement;
using Codecool.MarsExploration.MapExplorer.Pathfinder;
using Codecool.MarsExploration.MapExplorer.Pathfinder.Pathfinding;
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
    private IPathfinder _pathfinder;
    

    public ExplorationSimulator(IMapLoader mapLoader, IConfigurationValidator configurationValidator, IOutcomeAnalyzer lackOfResourcesAnalyzer, 
        IOutcomeAnalyzer succesAnalyzer, IOutcomeAnalyzer timeOutanalyzer, IRoverDeployer roverDeployer, 
        SimulationStepLoggingUi simulationStepLoggingUi, IGetLocationOfCommanCentre getLocationOfCommandCentre,
        IPathfinder pathfinder)
    {
        _mapLoader = mapLoader;
        _configurationValidator = configurationValidator;
        _lackOfResourcesAnalyzer = lackOfResourcesAnalyzer;
        _succesAnalyzer = succesAnalyzer;
        _timeOutanalyzer = timeOutanalyzer;
        _roverDeployer = roverDeployer;
        _simulationStepLoggingUi = simulationStepLoggingUi;
        _getLocationOfCommandCentre = getLocationOfCommandCentre;
        _pathfinder = pathfinder;
    }

    public void RunSimulation(Configuration.Configuration configuration)
    {
        var map = _mapLoader.Load(configuration.MapFile);
        
        //Landing spot for the spaceship
        var landingSpot = CheckLandingSpotForClear(configuration.LandingSpot, map);
        
        //Rover deployer in an adjacent coordinate
        var rover1 = _roverDeployer.Deploy();
        
        //Rover one begins its exploration routine
        var simulationContext = new SimulationContext(0, configuration.StepsToTimeOut, new List<Rover>{rover1},null, null,
            landingSpot, map, configuration.SymbolsOfTheResources);
        ExploringRoutine exploringRoutine = new ExploringRoutine(simulationContext);
        
        //Rover one finds a colonisable spot
        while (simulationContext.ExplorationOutcome != ExplorationOutcome.Colonizable)
        {
            simulationContext = SimulationLoop(new SimulationContext(0, configuration.StepsToTimeOut, new List<Rover>{rover1},null, null,
                landingSpot, map, configuration.SymbolsOfTheResources), exploringRoutine);
        }

        var colonizableSpot = simulationContext.Rovers.First().CurrentPosition;
        
        //Choose the location of the command centre and init 
        simulationContext = InitCommandCentre(colonizableSpot, simulationContext);
        _simulationStepLoggingUi.Run(simulationContext);
        
        //Rover one extracts minerals and gathers them at the command centre Coordinate
        FirstRoverPath(simulationContext.Rovers.First().CurrentPosition);

        //Build the centre


        //Build rover2

        //Rover2 extracts waters


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
            _simulationStepLoggingUi.Run(simulationContext);
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
                _simulationStepLoggingUi.Run(simulationContext);
            }

            simulationContext = simulationContext with { Step = simulationContext.Step + 1 };
            step++;
        }

        return simulationContext;
    }

    private SimulationContext InitCommandCentre(Coordinate spot, SimulationContext simulationContext)
    {
        var commandCentre = _getLocationOfCommandCentre.GetCentreLocation(spot, simulationContext);
        simulationContext = simulationContext with
        {
            CommandCenters = new List<CommandCenter.Model.CommandCenter> { commandCentre },
            ExplorationOutcome = ExplorationOutcome.InProgress
        };
        return simulationContext;
    }

    private void FirstRoverPath(Coordinate position)
    {
        Node start = new Node(true, position);
        Node target = new Node(true, position);
        var path = _pathfinder.FindPath(start, target);
        foreach (var node in path)
        {
            Console.WriteLine(node);
        }
    }
}