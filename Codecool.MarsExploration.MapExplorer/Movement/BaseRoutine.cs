using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.Movement;

public abstract class BaseRoutine
{
    private readonly CoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    
    private readonly SimulationContext _simulationContext;

    protected BaseRoutine(SimulationContext simulationContext)
    {
        _simulationContext = simulationContext;
    }
    
    public void Move(Rover rover,Coordinate coordinate)
    {
        rover.CurrentPosition = coordinate;
    }

    protected void Scan(Rover rover)
    {
        rover.VisibleTiles =ScanAdjacent(rover.CurrentPosition, 31);
        
        var map = _simulationContext.Map;
        
        var resources = rover.EncounteredResources.ToList();
        
        foreach (var visibleTile in rover.VisibleTiles)
        {
            if (_simulationContext.SymbolsToLookFor.Contains(map.Representation[visibleTile.X,visibleTile.Y]) && !resources.Contains(visibleTile))
            {
                resources.Add(visibleTile);
            }
        }

        rover.EncounteredResources = resources;
    }
    private IEnumerable<Coordinate> ScanAdjacent(Coordinate coordinate, int dimension)
    {
        List<Coordinate> coordinates = new List<Coordinate>();
        for (int i = coordinate.X -5; i <= coordinate.X+5; i++)
        {
            for (int j = coordinate.Y-5; j <= coordinate.Y+5; j++)
            {
                if (j >=0 && i >= 0 && j <= dimension-5 && i <= dimension-5 && !(j == coordinate.Y && i == coordinate.X))
                {
                    coordinates.Add(new Coordinate(i,j));
                }
            }
        }

        return coordinates;
    }
    public abstract void Step(Rover rover);
}