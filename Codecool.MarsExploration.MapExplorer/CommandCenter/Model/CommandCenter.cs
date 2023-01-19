using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.CommandCenter.Model;

public class CommandCenter
{
    public string Id { get; }
    
    public Coordinate CurrentPosition { get; }
    
    public int Radius { get; }
    
    public Status Status { get; set; }
    
    public int WaterResources { get; set; }
    
    public int MineralResources { get; set; }

    public CommandCenter(string id, Coordinate currentPosition, int radius, Status status, int waterResources,
        int mineralResources)
    {
        Id = id;
        CurrentPosition = currentPosition;
        Radius = radius;
        Status = status;
        WaterResources = waterResources;
        MineralResources = mineralResources;
    }
}