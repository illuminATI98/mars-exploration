using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder;

public interface AStarAlgorithm
{
    public int GCost(Coordinate startingNode, Coordinate targetNode);
    public int HCost(Coordinate endNode, Coordinate targetNode);
    public int FCost(int gCost, int hCost);
}