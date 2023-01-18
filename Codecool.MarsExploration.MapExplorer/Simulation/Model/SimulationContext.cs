using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using Codecool.MarsExploration.MapExplorer.Construction;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Model;

public record SimulationContext(int Step, int StepsToReachTimeOut, IEnumerable<Rover> Rovers,IEnumerable<CommandCenter.Model.CommandCenter> CommandCenters,IEnumerable<Construction.Model.Construction> Constructions,Coordinate LocationOfTheSpaceship, Map Map, IEnumerable<string> SymbolsToLookFor, ExplorationOutcome ExplorationOutcome = ExplorationOutcome.InProgress);
