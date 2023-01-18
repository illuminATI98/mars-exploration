using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.MarsRover.Service;

public interface IGetLocationOfCommanCentre
{
    Coordinate GetCentreLocation(Coordinate roverCurrentPosition, SimulationContext simulationContext);
}