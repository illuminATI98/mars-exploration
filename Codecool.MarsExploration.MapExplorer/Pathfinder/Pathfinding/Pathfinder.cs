using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder.Pathfinding;

public class Pathfinder : IPathfinder
{
    private readonly ICoordinateCalculator _coordinateCalculator;
    private readonly ICostCalculator _costCalculator;
    private readonly string[,] _map;
    private int _count;
    public Pathfinder(ICoordinateCalculator coordinateCalculator, string[,] map, ICostCalculator costCalculator)
    {
        _coordinateCalculator = coordinateCalculator;
        _map = map;
        _costCalculator = costCalculator;
    }

    public IEnumerable<Node>? FindPath(Node startingNode, Node targetNode)
    {
        List<Node> openNodes = new List<Node>();
        HashSet<Node> closedNodes = new HashSet<Node>();
        List<Node> path = new List<Node>();
        bool exit = false;
        openNodes.Add(startingNode);
        
        while (openNodes.Count > 0 && !exit)
        {
            Node currentNode = SetCurrentNode(openNodes[0], openNodes);

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            if (currentNode.MapPosition == targetNode.MapPosition)
            {
                path = RetracePath(startingNode, targetNode).ToList();
                exit = true;
            }
            
            foreach (var neighbour in GetNeighbours(currentNode))
            {
                var closeNodes = closedNodes.Where(n => n.MapPosition == neighbour.MapPosition).Any();
                if (!neighbour.Traversable || closeNodes)
                {
                    continue;
                }
                SetNeighbourCost(currentNode, neighbour, openNodes, targetNode);
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

    private void SetNeighbourCost(Node currentNode, Node neighbour, List<Node>openNodes, Node targetNode)
    {
        int movementCostToNeighbour = currentNode.GCost + _costCalculator.GetDistanceCost(currentNode, neighbour);
        var openMatches = openNodes.Where(n => n.MapPosition == neighbour.MapPosition).Any();
        if (movementCostToNeighbour < neighbour.GCost || !openMatches)
        {
            neighbour.GCost = movementCostToNeighbour;
            neighbour.HCost = _costCalculator.GetDistanceCost(neighbour, targetNode);
            neighbour.Parent = currentNode;
            var updatedMatch = openNodes.Where(n => n.MapPosition == neighbour.MapPosition).Any();
            if (!updatedMatch)
            {
                openNodes.Add(neighbour);
            }
            if (neighbour.MapPosition == targetNode.MapPosition)
            {
                targetNode.Parent = currentNode;
            }
            else
            {
                neighbour.Parent = currentNode;
            }
        }
    }

    private Node SetCurrentNode(Node starting, List<Node> openNodes)
    {
        Node currentNode = starting;
        for (int i = 0; i < openNodes.Count; i++)
        {
            if (openNodes[i].FCost < currentNode.FCost || openNodes[i].FCost == currentNode.FCost && openNodes[i].HCost < currentNode.HCost)
            {
                currentNode = openNodes[i];
            }
        }

        return currentNode;
    }
}