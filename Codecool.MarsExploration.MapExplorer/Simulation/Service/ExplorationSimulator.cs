using Codecool.MarsExploration.MapExplorer.Analyzer;
using Codecool.MarsExploration.MapExplorer.CommandCenter.Model;
using Codecool.MarsExploration.MapExplorer.Configuration.Service;
using Codecool.MarsExploration.MapExplorer.Construction.Service;
using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Movement;
using Codecool.MarsExploration.MapExplorer.Pathfinder;
using Codecool.MarsExploration.MapExplorer.Pathfinder.Pathfinding;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapExplorer.UI;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using Task = Codecool.MarsExploration.MapExplorer.MarsRover.Task;

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
    private ICoordinateCalculator _coordinateCalculator;
    private IBuilder _builder;
    private ILogger _logger;
    

    public ExplorationSimulator(IMapLoader mapLoader, IConfigurationValidator configurationValidator, IOutcomeAnalyzer lackOfResourcesAnalyzer, 
        IOutcomeAnalyzer succesAnalyzer, IOutcomeAnalyzer timeOutanalyzer, IRoverDeployer roverDeployer, 
        SimulationStepLoggingUi simulationStepLoggingUi, IGetLocationOfCommanCentre getLocationOfCommandCentre,
        IPathfinder pathfinder, ICoordinateCalculator coordinateCalculator,IBuilder builder, ILogger logger)
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
        _coordinateCalculator = coordinateCalculator;
        _builder = builder;
        _logger = logger;
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
        
        //Extract mineral
        simulationContext = ExtractResource(simulationContext, map, _coordinateCalculator, Task.MineralMining);
        
        //Build the centre
        simulationContext = _builder.Build(simulationContext, "center");
        
        //Build rover2
        simulationContext = _builder.Build(simulationContext, "rover");
        
        //Rover2 extracts waters
        simulationContext = ExtractResource(simulationContext, map, _coordinateCalculator, Task.WaterGathering);
        
        //END
        DisplayFinish(simulationContext, _logger);
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
                simulationContext = simulationContext with { Step = simulationContext.Step + 1 };
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

    private List<Coordinate> CollectedResource(SimulationContext simulationContext, Map map, string resource)
    {
        var mineralPlaces = new List<Coordinate>();
        foreach (var encounteredResource in simulationContext.Rovers.First().EncounteredResources)
        {
            if (map.Representation[encounteredResource.X, encounteredResource.Y] == resource)
            {
                mineralPlaces.Add(encounteredResource);
            }
        }

        return mineralPlaces;
    }

    private SimulationContext ExtractResource(SimulationContext simulationContext, Map map,ICoordinateCalculator coordinateCalculator ,Task task)
    {
        
        var miningRovers = simulationContext.Rovers.Where(rover => rover.Task == task);
        var rover = miningRovers.First();
        
        var resources = CollectedResource(simulationContext, map, task == Task.MineralMining ? "%" : "*");
        var resourceAmount = resources.Count;
        while (resources.Count > 0)
        {
            var resource = resources[Random.Next(0, resources.Count)];
            resources.Remove(resource);
            
            var adjacentMineralCoords =
                coordinateCalculator.GetAdjacentCoordinates(resource,
                    map.Representation.GetLength(0));
            var mineralCoordinate = adjacentMineralCoords.First(mineral => map.Representation[mineral.X, mineral.Y] == " ");

            _logger.TargetPosition(rover, simulationContext, mineralCoordinate);
            simulationContext = MoveToTarget(simulationContext, task, mineralCoordinate, rover, resourceAmount);
            _logger.Extracting(rover, simulationContext);
            RemoveCollectedFromMap(resource, simulationContext);
        }

        simulationContext = DeliverResource(simulationContext, rover);
        return simulationContext;
    }

    private SimulationContext DeliverResource(SimulationContext simulationContext,Rover rover)
    {
        ExploringRoutine exploringRoutine = new ExploringRoutine(simulationContext);
        var commandCenterPos = simulationContext.CommandCenters.First().CurrentPosition;
        var pathToCenter =
            _pathfinder.FindPath(new Node(true, rover.CurrentPosition), new Node(true, commandCenterPos));
        
        foreach (var node in pathToCenter)
        {
            exploringRoutine.Move(rover, node.MapPosition);
            simulationContext = simulationContext with
            {
                Rovers = new List<Rover>{rover},
                Step = simulationContext.Step + 1,
            };
            _logger.Position(rover, simulationContext);
        }
        var updateRover = simulationContext.Rovers.ToList();
        foreach (var rover1 in updateRover)
        {
            if (rover1 == rover)
            {
                rover1.CurrentRoutine = Routine.Delivering;
            }
        }

        var updateCenter = simulationContext.CommandCenters.ToList();
        foreach (var commandCenter in updateCenter)
        {
            if (commandCenter == simulationContext.CommandCenters.First())
            {
                if (rover.Task == Task.MineralMining)
                {
                    commandCenter.MineralResources = rover.Progress;
                }

                if (rover.Task == Task.WaterGathering)
                {
                    commandCenter.WaterResources = rover.Progress;
                }
            }
        }
        
        simulationContext = simulationContext with
        {
            Rovers = updateRover
        };
        _logger.Delivering(rover,simulationContext);
        return simulationContext;
    }
    
    private SimulationContext MoveToTarget(SimulationContext simulationContext, Task task, Coordinate coordinate, Rover rover, int maxMinerals)
    {
        ExploringRoutine exploringRoutine = new ExploringRoutine(simulationContext);
        var pathToMineral = _pathfinder.FindPath(new Node(true, rover.CurrentPosition), new Node(true, coordinate));
        
        foreach (var node in pathToMineral)
        {
            exploringRoutine.Move(rover, node.MapPosition);
            simulationContext = simulationContext with
            {
                Rovers = new List<Rover>{rover},
                Step = simulationContext.Step + 1,
            };
            _logger.Position(rover, simulationContext);
        }

        var updateRover = simulationContext.Rovers.ToList();
        
        foreach (var rover1 in updateRover)
        {
            if (rover1 == rover)
            {
                rover1.CurrentRoutine = Routine.Extracting;
                rover1.Resource = task == Task.MineralMining ? "MINERAL" : "WATER";
                rover1.Progress++;
                rover1.ResourcesToMine = maxMinerals;
            }
        }
        
        simulationContext = simulationContext with
        {
            Rovers = updateRover
        };
        
        return simulationContext;
    }

    private void RemoveCollectedFromMap(Coordinate item, SimulationContext simulationContext)
    {
        simulationContext.Map.Representation[item.X, item.Y] = " ";
    }

    private void DisplayFinish(SimulationContext simulationContext, ILogger logger)
    {
        simulationContext = simulationContext with
        {
            Step = simulationContext.Step + 1
        };
        
        logger.Final(simulationContext);
    }
}