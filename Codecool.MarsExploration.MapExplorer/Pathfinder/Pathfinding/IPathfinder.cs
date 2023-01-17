using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Pathfinder.Pathfinding;

public interface IPathfinder
{
    IEnumerable<Coordinate> FindPath(Coordinate startingPos, Coordinate targetPos);
}