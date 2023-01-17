using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder;

public class CostCalculator : ICostCalculator
{
    public int GCost(Coordinate startingNode, Coordinate targetNode)
    {
        return PythagorasDistance(startingNode, targetNode);
    }

    public int HCost(Coordinate endNode, Coordinate targetNode)
    {
        return PythagorasDistance(endNode, targetNode);
    }

    public int FCost(int gCost, int hCost)
    {
        return gCost + hCost;
    }

    private int PythagorasDistance(Coordinate startingNode, Coordinate targetNode)
    {
        int xDistance = Math.Abs(startingNode.X - targetNode.X);
        int yDistance = Math.Abs(startingNode.Y - targetNode.Y);

        double distance = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));

        return (int)Math.Round(distance, 0, MidpointRounding.AwayFromZero) * 10;
    }
}