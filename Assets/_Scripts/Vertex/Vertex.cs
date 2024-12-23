using System.Collections.Generic;
using UnityEngine;

public class Vertex 
{
    public Vector3Int Coordinats { get; private set; }
    public float Activation { get; private set; }
    public List<Chunk> ParentChunks { get; private set; } = new();

    public bool IsEdgeVertex { get; set; }

    public Vertex(Vector3Int coordinats, float activation, bool isEdgeVertex = false) 
    {
        Coordinats = coordinats;
        IsEdgeVertex = isEdgeVertex;
        Activation = activation;
    }

    public void AddParentChunkLink(Chunk chunk) => ParentChunks.Add(chunk);

    public void UpdateActivation(float activation) => Activation = activation;

    public void AddActivation(float activationIncrement) => Activation += activationIncrement;
}