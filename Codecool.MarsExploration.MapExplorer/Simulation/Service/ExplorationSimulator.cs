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
        
        
        //Rover one extracts minerals and gathers them at the command centre Coordinate
        simulationContext = FirstRoverPath(simulationContext, map, _coordinateCalculator);
        
        //Build the centre
        simulationContext = _builder.Build(simulationContext, "center");
        
        //Build rover2
        simulationContext = _builder.Build(simulationContext, "rover");
        
        //Rover2 extracts waters
        simulationContext = SecondRoverPath(simulationContext, map, _coordinateCalculator);
        
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

    private List<Coordinate> CollectedMinerals(SimulationContext simulationContext, Map map)
    {
        var mineralPlaces = new List<Coordinate>();
        foreach (var encounteredResource in simulationContext.Rovers.First().EncounteredResources)
        {
            if (map.Representation[encounteredResource.X, encounteredResource.Y] == "%")
            {
                mineralPlaces.Add(encounteredResource);
            }
        }

        return mineralPlaces;
    }

    private SimulationContext FirstRoverPath(SimulationContext simulationContext, Map map, ICoordinateCalculator coordinateCalculator)
    {
        Node started = new Node(true, simulationContext.Rovers.First().CurrentPosition);
        var mineralPlaces = CollectedMinerals(simulationContext, map);

        foreach (var mineralPlace in mineralPlaces)
        {
             Node start = new Node(true, simulationContext.Rovers.First().CurrentPosition);
            var possibleTargetCoordinates = coordinateCalculator.GetAdjacentCoordinates(mineralPlace, 32).ToList();
            var randomCor = Random.Next(possibleTargetCoordinates.Count);
            
            Node target = new Node(true, possibleTargetCoordinates[randomCor]);

            var path = _pathfinder.FindPath(start, target);
            
            var changeRover = new Rover(simulationContext.Rovers.First().Id, target.MapPosition,
                simulationContext.Rovers.First().VisibleTiles, simulationContext.Rovers.First().EncounteredResources,
                Routine.Extracting, "mineral", 1);

            var updatedCentre = new CommandCenter.Model.CommandCenter(simulationContext.CommandCenters.First().Id,
                simulationContext.CommandCenters.First().CurrentPosition,
                simulationContext.CommandCenters.First().Radius, Status.Expanding, null, mineralPlaces);
            
            simulationContext = simulationContext with
            {
                Rovers = new List<Rover>{changeRover},
                Step = simulationContext.Step + 1,
                CommandCenters = new List<CommandCenter.Model.CommandCenter>{updatedCentre}
            };
            
            RemoveCollectedFromMap( mineralPlace, simulationContext);
            
            _simulationStepLoggingUi.Run(simulationContext);
            
        }
        
        simulationContext = simulationContext with
        {
            Step = simulationContext.Step + 1
        };

        Node finalPosition = new Node(true, simulationContext.Rovers.First().CurrentPosition);
        _pathfinder.FindPath(finalPosition, started);
        
        var changeRover1 = new Rover(simulationContext.Rovers.First().Id, simulationContext.Rovers.First().CurrentPosition,
            simulationContext.Rovers.First().VisibleTiles, simulationContext.Rovers.First().EncounteredResources,
            Routine.Delivering, "mineral", 2);
        simulationContext = simulationContext with
        {
            Rovers = new List<Rover>{changeRover1}
        };
        
        _simulationStepLoggingUi.Run(simulationContext);
        return simulationContext;
    }

    private void RemoveCollectedFromMap(Coordinate item, SimulationContext simulationContext)
    {
        simulationContext.Map.Representation[item.X, item.Y] = " ";
    }

    private List<Coordinate> CollectedWaters(SimulationContext simulationContext, Map map)
    {
        var waterPlaces = new List<Coordinate>();
        foreach (var encounteredResource in simulationContext.Rovers.First().EncounteredResources)
        {
            if (map.Representation[encounteredResource.X, encounteredResource.Y] == "*")
            {
                waterPlaces.Add(encounteredResource);
            }
        }

        return waterPlaces;
    }

    private SimulationContext SecondRoverPath(SimulationContext simulationContext, Map map,
        ICoordinateCalculator coordinateCalculator)
    {
        var firstRover = simulationContext.Rovers.First();
        var secondRover = simulationContext.Rovers.First(x => x.Id == simulationContext.Construction.UnitId);
        
        Node started = new Node(true, secondRover.CurrentPosition);
        var waterPlaces = CollectedWaters(simulationContext, map);
        
        foreach (var waterPlace in waterPlaces)
        {
            Node start = new Node(true, simulationContext.Rovers.First(x => x.Id == simulationContext.Construction.UnitId).CurrentPosition);
            var possibleTargetCoordinates = coordinateCalculator.GetAdjacentCoordinates(waterPlace, 32).ToList();
            var randomCor = Random.Next(possibleTargetCoordinates.Count);
            
            Node target = new Node(true, possibleTargetCoordinates[randomCor]);

            var path = _pathfinder.FindPath(start, target);

            var changeRover = new Rover(simulationContext.Rovers.First(x => x.Id == simulationContext.Construction.UnitId).Id, target.MapPosition,
                simulationContext.Rovers.First(x => x.Id == simulationContext.Construction.UnitId).VisibleTiles, simulationContext.Rovers.First(x => x.Id == simulationContext.Construction.UnitId).EncounteredResources,
                Routine.Extracting, "water", 1);
            
            var updatedCentre = new CommandCenter.Model.CommandCenter(simulationContext.CommandCenters.First().Id,
                simulationContext.CommandCenters.First().CurrentPosition,
                simulationContext.CommandCenters.First().Radius, Status.Expanding, waterPlaces,
                simulationContext.CommandCenters.First().MineralResources);

            simulationContext = simulationContext with
            {
                Rovers = new List<Rover>{changeRover},
                Step = simulationContext.Step + 1,
                CommandCenters = new List<CommandCenter.Model.CommandCenter>{updatedCentre}
            };
            
            RemoveCollectedFromMap( waterPlace, simulationContext);
            
            _simulationStepLoggingUi.Run(simulationContext);
            
        }
        
        simulationContext = simulationContext with
        {
            Step = simulationContext.Step + 1
        };
        
        Node finalPosition = new Node(true, simulationContext.Rovers.First(x => x.Id == simulationContext.Construction.UnitId).CurrentPosition);
        _pathfinder.FindPath(finalPosition, started);
        
        var changeRover2 = new Rover(simulationContext.Rovers.First(x => x.Id == simulationContext.Construction.UnitId).Id, simulationContext.Rovers.First(x => x.Id == simulationContext.Construction.UnitId).CurrentPosition,
            simulationContext.Rovers.First(x => x.Id == simulationContext.Construction.UnitId).VisibleTiles, simulationContext.Rovers.First(x => x.Id == simulationContext.Construction.UnitId).EncounteredResources,
            Routine.Delivering, "water", 2);
        
        simulationContext = simulationContext with
        {
            Rovers = new List<Rover>{changeRover2},
            Step = simulationContext.Step + 1
        };

        _simulationStepLoggingUi.Run(simulationContext);
        simulationContext = simulationContext with
        {
            Rovers = new List<Rover>{firstRover, changeRover2}
        };
        return simulationContext;
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