namespace Codecool.MarsExploration.MapExplorer.Pathfinder.Optimisation;

public interface IHeap<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}