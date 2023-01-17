using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.CommandCenter.Model;

public class CommandCenter
{
    public string Id { get; set; }
    
    public Coordinate CurrentPosition { get; set; }
    
    public int Radius { get; set; }
    
    public Status Status { get; set; }

    public CommandCenter(string id, Coordinate currentPosition, int radius, Status status)
    {
        Id = id;
        CurrentPosition = currentPosition;
        Radius = radius;
        Status = status;
    }
}