using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Logger;

public interface ILogger
{
    void Log(string message);
    void Position(Rover rover);
    void Position(CommandCenter.Model.CommandCenter commandCenter);
    void Step(SimulationContext simulationContext);
    void OutCome(SimulationContext simulationContext);
    void Construction(Construction.Model.Construction simulationContext);
}
