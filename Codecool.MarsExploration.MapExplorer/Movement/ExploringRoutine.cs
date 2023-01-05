using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.Movement;

public class ExploringRoutine : BaseRoutine
{
    private readonly CoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    private readonly SimulationContext _simulationContext;
    private static readonly Random Random = new();
    
    public ExploringRoutine(SimulationContext simulationContext): base(simulationContext)
    {
        _simulationContext = simulationContext;
    }

    public override void Step(Rover rover)
    {
        var emptyTiles = new List<Coordinate>();

        var map = _simulationContext.Map;
        
        foreach (var visibleTile in rover.VisibleTiles)
        {
            if (map.IsEmpty(visibleTile))
            {
                emptyTiles.Add(visibleTile);
            }
        }
        var randomCoordinate = GetTargetCoordinate(emptyTiles);
        
        Move(rover, randomCoordinate);
        Scan(rover);
    }
    
    private Coordinate GetTargetCoordinate(List<Coordinate> coordinates)
    {
        return coordinates.Any()
            ? coordinates[Random.Next(coordinates.Count)]
            : _coordinateCalculator.GetRandomCoordinate(1);
    }
}