using UnityEngine;

public class Vertex 
{
    public VertexType Type { get; set; }
    
    public Vector3 Coordinats { get; private set; }
    public float Activation { get; private set; }

    public bool IsEdgeVertex { get; set; }

    public Vertex(Vector3 coordinats, VertexType vertexType, float activation, bool isEdgeVertex = false) 
    {
        Coordinats = coordinats;
        IsEdgeVertex = isEdgeVertex;
        Activation = activation;
        Type = vertexType;
    }

    public void UpdateActivation(float activation) => Activation = activation;

    public void AddActivation(float activationIncrement) 
    {
        if (Type == VertexType.Bedrock) return;

        Activation += activationIncrement;
    } 

    public void AddActivation(float activationIncrement, VertexType placingVertexType) 
    {
        if (Type == VertexType.Bedrock) return;

        Activation += activationIncrement;

        if (activationIncrement <= 0) return;

        Type = placingVertexType;
    }
}
