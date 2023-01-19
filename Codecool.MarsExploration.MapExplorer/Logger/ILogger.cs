using Codecool.MarsExploration.MapExplorer.MarsRover;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Logger;

public interface ILogger
{
    void Log(string message);
    void Position(Rover rover,SimulationContext simulationContext);
    public void TargetPosition(Rover rover, SimulationContext simulationContext, Coordinate coordinate);
    void Extracting(Rover rover,SimulationContext simulationContext);
    void Delivering(Rover rover,SimulationContext simulationContext);
    void Position(CommandCenter.Model.CommandCenter commandCenter,SimulationContext simulationContext);
    void OutCome(SimulationContext simulationContext);
    void Construction(SimulationContext simulationContext);

    void Final(SimulationContext simulationContext);
}
