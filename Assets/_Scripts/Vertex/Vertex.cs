using System.Collections.Generic;
using UnityEngine;

public class Vertex 
{
    public VertexType Type { get; set; }

    public Vector3Int Coordinats { get; private set; }
    public float Activation { get; private set; }
    public List<ChunkAlloc> ParentChunks { get; private set; } = new();

    public bool IsEdgeVertex { get; set; }

    public Vertex(Vector3Int coordinats, VertexType vertexType, float activation, bool isEdgeVertex = false) 
    {
        Coordinats = coordinats;
        IsEdgeVertex = isEdgeVertex;
        Activation = activation;
        Type = vertexType;
    }

    public void AddParentChunkLink(ChunkAlloc chunk) => ParentChunks.Add(chunk);

    public void UpdateActivation(float activation) => Activation = activation;

    public void AddActivation(float activationIncrement) => Activation += activationIncrement;
}