using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder;

public class Node
{
    public bool Traversable;
    public readonly Coordinate MapPosition;
    public Node Parent;
    public int GCost { get; set;}
    public int HCost { get; set;}
    public int FCost => GCost + HCost;
    public Node(bool traversable, Coordinate mapPosition)
    {
        Traversable = traversable;
        MapPosition = mapPosition;
    }
}