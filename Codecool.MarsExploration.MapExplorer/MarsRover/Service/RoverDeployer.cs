using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using NotImplementedException = System.NotImplementedException;

namespace Codecool.MarsExploration.MapExplorer.MarsRover.Service;

public class RoverDeployer : IRoverDeployer
{
    private readonly CoordinateCalculator _coordinateCalculator;
    
    private static readonly Random Random = new();

    private readonly Configuration.Configuration _configuration;
    
    private readonly MapLoader.MapLoader _mapLoader;

    public RoverDeployer(CoordinateCalculator coordinateCalculator, Configuration.Configuration configuration, MapLoader.MapLoader mapLoader)
    {
        _coordinateCalculator = coordinateCalculator;
        _configuration = configuration;
        _mapLoader = mapLoader;
    }
    public Rover Deploy()
    {
        int count = 0;
        var adjacentCoordinates = _coordinateCalculator.GetAdjacentCoordinates(_configuration.LandingSpot, 31).ToList();
        var visibleTiles = GetVisibleTiles(GetTargetCoordinate(adjacentCoordinates));
        var encounteredResources = new List<Coordinate>();
        var map = _mapLoader.Load(_configuration.MapFile);
        
        foreach (var visibleTile in visibleTiles)
        {
            if (_configuration.SymbolsOfTheResources.Contains(map.Representation[visibleTile.X,visibleTile.Y]))
            {
                encounteredResources.Add(visibleTile);
            }
        }

        return new Rover($"MER-B Opportunity-{count += 1}", GetTargetCoordinate(adjacentCoordinates), visibleTiles ,encounteredResources, Routine.Exploring);
    }
    
    private Coordinate GetTargetCoordinate(List<Coordinate> coordinates)
    {
        var randomCoord = Random.Next(coordinates.Count);
        return coordinates[randomCoord];
    }
    
    private IEnumerable<Coordinate> GetVisibleTiles(Coordinate coordinate)
    {
        return _coordinateCalculator.GetAdjacentCoordinates(coordinate, 9);
    }
}