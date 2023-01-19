using Codecool.MarsExploration.MapExplorer.CommandCenter.Model;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.MarsRover.Service;

public class GetLocationOfCommandCentre : IGetLocationOfCommanCentre
{
    private readonly CoordinateCalculator _coordinateCalculator;
    
    private static readonly Random Random = new();

    private readonly Configuration.Configuration _configuration;
    
    private readonly MapLoader.MapLoader _mapLoader;
    
    public GetLocationOfCommandCentre(CoordinateCalculator coordinateCalculator, Configuration.Configuration configuration, MapLoader.MapLoader mapLoader)
    {
        _coordinateCalculator = coordinateCalculator;
        _configuration = configuration;
        _mapLoader = mapLoader;
    }
    
    public CommandCenter.Model.CommandCenter GetCentreLocation(Coordinate roverCurrentPosition, SimulationContext simulationContext)
    {
        var adjacentCoords = GetVisibleTiles(roverCurrentPosition, simulationContext);
        var emptyTiles = new List<Coordinate>();
        var map = simulationContext.Map;

        foreach (var adjacentCoord in adjacentCoords)
        {
            if (map.Representation[adjacentCoord.X, adjacentCoord.Y] == " " && adjacentCoord != roverCurrentPosition)
            {
                emptyTiles.Add(adjacentCoord);   
            }
        }
        
        var randomCoordinate = GetTargetCoordinate(emptyTiles);
        return new CommandCenter.Model.CommandCenter("CENTER-1", randomCoordinate, 1, Status.Expanding, 0, 0);
    }
    
    private List<Coordinate> GetVisibleTiles(Coordinate coordinate, SimulationContext simulationContext)
    {
        return _coordinateCalculator.GetAdjacentCoordinates(coordinate, simulationContext.Map.Representation.GetLength(0)).ToList();
    }
    
    private Coordinate GetTargetCoordinate(List<Coordinate> coordinates)
    {
        var randomCoord = Random.Next(coordinates.Count);
        return coordinates[randomCoord];
    }
}