using System.Collections.Immutable;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Construction.Service;

public interface IBuilder
{
    SimulationContext Build(SimulationContext simulationContext,string unit);
}