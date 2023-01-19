﻿using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.MarsRover;

public class Rover
{
    public string Id { get; set; }
    public Coordinate CurrentPosition { get; set; }
    public IEnumerable<Coordinate> VisibleTiles { get; set; }
    public IEnumerable<Coordinate> EncounteredResources { get; set; }
    public Routine CurrentRoutine { get; set; }
    
    public string Resource { get; set; }
    
    public int Progress { get; set; }

    public Rover(string id, Coordinate currentPosition, IEnumerable<Coordinate> visibleTiles, IEnumerable<Coordinate> encounteredResources, Routine currentRoutine, string resource, int progress)
    {
        Id = id;
        CurrentPosition = currentPosition;
        VisibleTiles = visibleTiles;
        EncounteredResources = encounteredResources;
        CurrentRoutine = currentRoutine;
        Resource = resource;
        Progress = progress;
    }
}
