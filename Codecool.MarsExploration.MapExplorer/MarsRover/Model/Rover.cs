using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.MarsRover;

public class Rover
{
    public string Id { get; set; }
    public Coordinate CurrentPosition { get; set; }
    public IEnumerable<Coordinate> VisibleTiles { get; set; }
    public IEnumerable<Coordinate> EncounteredResources { get; set; }
    public Routine CurrentRoutine { get; set; }
    public Task Task { get; set; }
    public string Resource { get; set; }
    public int ResourcesToMine { get; set; }
    public int Progress { get; set; }

    public Rover(string id, Coordinate currentPosition, IEnumerable<Coordinate> visibleTiles, IEnumerable<Coordinate> encounteredResources, Routine currentRoutine, string resource, int progress, Task task, int resourcesToMine)
    {
        Id = id;
        CurrentPosition = currentPosition;
        VisibleTiles = visibleTiles;
        EncounteredResources = encounteredResources;
        CurrentRoutine = currentRoutine;
        Resource = resource;
        Progress = progress;
        Task = task;
        ResourcesToMine = resourcesToMine;
    }
}
