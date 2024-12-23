using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3Int ChunkPositionInWorldSpace { get; private set; }

    public List<Vertex> Vertices { get; private set; } = new();

    [SerializeField]
    private ChunkMeshRenderer _meshRenderer;

    public void Init(Vector3Int chunkPosition)
    {
        ChunkPositionInWorldSpace = chunkPosition;
    }

    private List<Vector4> _convertVerticesToActivationValuesList(List<Vertex> vertices)
    {
        return vertices.Select(vertex => new Vector4(vertex.Coordinats.x - ChunkPositionInWorldSpace.x, vertex.Coordinats.y - ChunkPositionInWorldSpace.y, vertex.Coordinats.z - ChunkPositionInWorldSpace.z, vertex.Activation)).ToList();;
    }

    public void GenerateChunkMesh(ComputeCubes computeCubes)
    {
        List<Vector3> vertices = computeCubes.ComputeTriangleVertices(_convertVerticesToActivationValuesList(Vertices));

        if (vertices.Count == 0) 
        {
            _meshRenderer.ClearMesh();

            return;
        }

        ChunkMeshData meshData = new();

        meshData.PushVertices(vertices.ToList());

        _meshRenderer.RenderMesh(meshData);
    }

    public void LinkVertices(List<Vertex> vertices)
    {
        Vertices = vertices;
    }

    public void AddVertexLink(Vertex vertex)
    {
        Vertices.Add(vertex);
    }
}
