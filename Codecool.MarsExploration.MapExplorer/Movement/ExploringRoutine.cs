using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

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
        var adjacentCoords = _coordinateCalculator.GetAdjacentCoordinates(rover.CurrentPosition, _simulationContext.Map.Representation.GetLength(0));
        var emptyTiles = new List<Coordinate>();
        var map = _simulationContext.Map;

        foreach (var adjacentCoord in adjacentCoords)
        {
            if (map.Representation[adjacentCoord.X, adjacentCoord.Y] == " " && adjacentCoord != rover.CurrentPosition)
            {
                emptyTiles.Add(adjacentCoord);   
            }
        }
        
        var randomCoordinate = GetTargetCoordinate(emptyTiles);
        
        Move(rover, randomCoordinate);
        Scan(rover);
    }
    
    private Coordinate GetTargetCoordinate(List<Coordinate> coordinates)
    {
        var randomCoord = Random.Next(coordinates.Count);

        return coordinates[randomCoord];
    }
    
    private static bool CanMove(string?[,] map, Coordinate coordinate)
    {
        return coordinate.X >= 0
               && coordinate.X < map.GetLength(0)
               && coordinate.Y >= 0
               && coordinate.Y < map.GetLength(1)
               && map[coordinate.X, coordinate.Y] == " ";
    }

    public void MoveRover(Rover rover, Coordinate coordinate)
    {
        Move(rover, coordinate);
    }
}