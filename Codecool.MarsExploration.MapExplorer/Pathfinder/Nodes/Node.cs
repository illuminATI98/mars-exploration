using Codecool.MarsExploration.MapExplorer.Pathfinder.Optimisation;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder;

public class Node : IHeap<Node>
{
    public bool Traversable;
    public readonly Coordinate MapPosition;
    public Node Parent;
    public int GCost { get; set;}
    public int HCost { get; set;}
    public int FCost => GCost + HCost;
    private int _heapIndex;
    public Node(bool traversable, Coordinate mapPosition)
    {
        Traversable = traversable;
        MapPosition = mapPosition;
    }

    public int CompareTo(Node? nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare.HCost);
        }

        return -compare;
    }

    public int HeapIndex
    {
        get { return _heapIndex;}
        set { _heapIndex = value; }
    }
}