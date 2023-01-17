using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder;

public class Nodes
{
    private Node[,] _nodes;
    
    public void CreateNodeArray(string[,] map, int mapSize)
    {
        _nodes = new Node[mapSize, mapSize];

        for (var i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                _nodes[i, j] = CreateNode(i, j, map[i, j]);
            }
        }
    }
    
    private Node CreateNode(int xCoord, int yCoord, string representation)
    {
        return new Node(representation == " ", new Coordinate(xCoord, yCoord));
    }
}