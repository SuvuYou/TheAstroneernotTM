using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkMeshData
{
    public List<Vector3> Vertices { get; private set; } = new ();
    public List<int> Triangles { get => _generateTriangles(Vertices); }

    public void PushVertices(List<Vector3> vertices)
    {
        Vertices.AddRange(vertices);
    }

    private List<int> _generateTriangles(List<Vector3> vertices)
    {
        return Enumerable.Range(0, vertices.Count).ToList();
    }
}
