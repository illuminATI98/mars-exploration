using Codecool.MarsExploration.MapExplorer.Pathfinder.Optimisation;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder.Pathfinding;

public class Pathfinder : IPathfinder
{
    private readonly ICoordinateCalculator _coordinateCalculator;
    private readonly ICostCalculator _costCalculator;
    private readonly string[,] _map;
    
    public Pathfinder(ICoordinateCalculator coordinateCalculator, string[,] map, ICostCalculator costCalculator)
    {
        _coordinateCalculator = coordinateCalculator;
        _map = map;
        _costCalculator = costCalculator;
    }

    public IEnumerable<Node>? FindPath(Node startingNode, Node targetNode)
    {
        Heap<Node> openNodes = new Heap<Node>(_map.GetLength(0));
        HashSet<Node> closedNodes = new HashSet<Node>();
        List<Node> path = new List<Node>();
        bool exit = false;
        openNodes.Add(startingNode);

        while (openNodes.Count() > 0 && !exit)
        {
            Node currentNode = openNodes.RemoveFirst();
            closedNodes.Add(currentNode);

            if (currentNode.MapPosition == targetNode.MapPosition)
            {
                path = RetracePath(startingNode, targetNode).ToList();
                exit = true;
            }
            
            foreach (var neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.Traversable && !closedNodes.Contains(neighbour))
                {
                    continue;
                }
                int movementCostToNeighbour = currentNode.GCost + _costCalculator.GetDistanceCost(currentNode, neighbour);
                if (movementCostToNeighbour < neighbour.GCost || !openNodes.Contains(neighbour))
                {
                    neighbour.GCost = movementCostToNeighbour;
                    neighbour.HCost = _costCalculator.GetDistanceCost(neighbour, targetNode);
                    if (neighbour.MapPosition == targetNode.MapPosition)
                    {
                        targetNode.Parent = currentNode;
                    }
                    else
                    {
                        neighbour.Parent = currentNode;
                    }
                    if (!openNodes.Contains(neighbour))
                    {
                        openNodes.Add(neighbour);
                    }
                }
            }
        }

        return path;
    }

    private IEnumerable<Node> GetNeighbours(Node node)
    {
        IEnumerable<Coordinate> neighbouringCoords = _coordinateCalculator.GetAdjacentCoordinates(node.MapPosition, _map.GetLength(0));
        return neighbouringCoords.Select(neighbouringCoord => new Node(_map[neighbouringCoord.X, neighbouringCoord.Y] == " ", neighbouringCoord)).ToList();
    }

    private IEnumerable<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (startNode != currentNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }
    
}