using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder;

public interface ICostCalculator
{
    public int GetDistanceCost(Node nodeA, Node nodeB);

}