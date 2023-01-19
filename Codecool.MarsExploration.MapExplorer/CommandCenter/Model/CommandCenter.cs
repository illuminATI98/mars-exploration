using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.CommandCenter.Model;

public class CommandCenter
{
    public string Id { get; }
    
    public Coordinate CurrentPosition { get; }
    
    public int Radius { get; }
    
    public Status Status { get; set; }
    
    public List<Coordinate> WaterResources { get; set; }
    
    public List<Coordinate> MineralResources { get; set; }

    public CommandCenter(string id, Coordinate currentPosition, int radius, Status status, List<Coordinate> waterResources,
        List<Coordinate> mineralResources)
    {
        Id = id;
        CurrentPosition = currentPosition;
        Radius = radius;
        Status = status;
        WaterResources = waterResources;
        MineralResources = mineralResources;
    }
}