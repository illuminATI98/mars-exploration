using Codecool.MarsExploration.MapExplorer.Analyzer;
using Codecool.MarsExploration.MapExplorer.Configuration.Service;
using Codecool.MarsExploration.MapExplorer.Construction.Service;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Movement;
using Codecool.MarsExploration.MapExplorer.Pathfinder;
using Codecool.MarsExploration.MapExplorer.Pathfinder.Pathfinding;
using Codecool.MarsExploration.MapExplorer.Simulation.Service;
using Codecool.MarsExploration.MapExplorer.UI;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer;

class Program
{
    private static readonly string WorkDir = AppDomain.CurrentDomain.BaseDirectory;

    public static void Main(string[] args)
    {
        var mapFile = $@"{WorkDir}\Resources\exploration-0.map";
        var resources = new List<string>() { "*", "%" };
        const int stepsToTimeout = 100;
        var landingSpot = new Coordinate(6, 6);

        var configObject = new Configuration.Configuration(mapFile, landingSpot, resources, stepsToTimeout);

        MapLoader.MapLoader mapLoader = new MapLoader.MapLoader();
        CoordinateCalculator coordinateCalculator = new CoordinateCalculator();
        IConfigurationValidator configurationValidator = new ConfigurationValidator();
        IOutcomeAnalyzer successAnalyzer = new SuccessAnalyzer();
        IOutcomeAnalyzer timeOutAnalyzer = new TimeOutAnalyzer();
        IOutcomeAnalyzer lackOfResourcesAnalyzer = new LackOfResourcesAnalyzer();
        IRoverDeployer roverDeployer = new RoverDeployer(coordinateCalculator,configObject,mapLoader);
        ILogger logger = new ConsoleLogger();
        SimulationStepLoggingUi simulationStepLoggingUi = new SimulationStepLoggingUi(logger);
        IGetLocationOfCommanCentre getLocationOfCommanCentre = new GetLocationOfCommandCentre(coordinateCalculator,configObject,mapLoader);
        ICostCalculator costCalculator = new CostCalculator();
        IPathfinder pathfinder = new Pathfinder.Pathfinding.Pathfinder(coordinateCalculator,
            mapLoader.Load(mapFile).Representation, costCalculator);
        IBuilder builder = new Builder(logger); 

        IExplorationSimulator explorationSimulator = new ExplorationSimulator(mapLoader, configurationValidator, lackOfResourcesAnalyzer,successAnalyzer,timeOutAnalyzer,roverDeployer, 
            simulationStepLoggingUi, getLocationOfCommanCentre, pathfinder, coordinateCalculator,builder, logger);
        
        explorationSimulator.RunSimulation(configObject);
    }
}
