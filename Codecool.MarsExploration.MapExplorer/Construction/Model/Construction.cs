using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Construction.Model;

public class Construction
{
    public string UnitId { get; }
    
    public Coordinate Position { get; }
    
    public int Progress { get; set; }
    
    public int StepsToComplete { get; }

    public Construction(string unitId, Coordinate position, int progress, int stepsToComplete)
    {
        UnitId = unitId;
        Position = position;
        Progress = progress;
        StepsToComplete = stepsToComplete;
    }
}