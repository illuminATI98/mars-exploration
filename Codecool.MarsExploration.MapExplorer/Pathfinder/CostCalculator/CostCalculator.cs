using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder;

public class CostCalculator : ICostCalculator
{
    private int DiagonalDistance()
    {
        return PythagorasDistance(new Coordinate(0, 0), new Coordinate(1, 1));
    }
    private int HorizontalDistance()
    {
        return PythagorasDistance(new Coordinate(0, 0), new Coordinate(0, 1));
    }
    private int PythagorasDistance(Coordinate startingNode, Coordinate targetNode)
    {
        int xDistance = Math.Abs(startingNode.X - targetNode.X);
        int yDistance = Math.Abs(startingNode.Y - targetNode.Y);
        
        double distance = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));

        return (int)Math.Round(distance * 10, 0, MidpointRounding.AwayFromZero);
    }
    
    public int GetDistanceCost(Node nodeA, Node nodeB)
    {
        int distX = Math.Abs(nodeA.MapPosition.X - nodeB.MapPosition.X);
        int distY = Math.Abs(nodeA.MapPosition.Y - nodeB.MapPosition.Y);

        int horizontalDistance = HorizontalDistance();
        int diagonalDistance = DiagonalDistance();

        if (distX > distY)
        {
            return diagonalDistance * distY + horizontalDistance * (distX - distY);
        }
        return diagonalDistance * distX + horizontalDistance * (distY - distX);
    }
}