using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder.Pathfinding;

public interface IPathfinder
{
    IEnumerable<Node>? FindPath(Node startingPos, Node targetPos);
}