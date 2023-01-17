using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder;

public class Node
{
    private bool _traversable;
    private Coordinate _mapPosition;


    public Node(bool traversable, Coordinate mapPosition)
    {
        _traversable = traversable;
        _mapPosition = mapPosition;
    }
}