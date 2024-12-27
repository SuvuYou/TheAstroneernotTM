using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkAlloc : MonoBehaviour
{
    public Vector3Int ChunkPositionInWorldSpace { get; private set; }

    public Vertex[] Vertices { get; private set; } = new Vertex[0];
    public Vector4[] VerticesActivations { get; private set; } = new Vector4[0];

    [SerializeField]
    private ChunkMeshRenderer _meshRenderer;

    public void Init(Vector3Int chunkPosition)
    {
        Vertices  = new Vertex[WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS * WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS];
        VerticesActivations = new Vector4[WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS * WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS];
        ChunkPositionInWorldSpace = chunkPosition;
    }

    private void _convertVerticesToActivationValuesList()
    {
        for (int i = 0; i < Vertices.Length; i++)
        {
            if (Vertices[i] == null) 
            {
                // Debug.Log(i);

                continue;
            }

            VerticesActivations[i].x = Vertices[i].Coordinats.x - ChunkPositionInWorldSpace.x;
            VerticesActivations[i].y = Vertices[i].Coordinats.y - ChunkPositionInWorldSpace.y;
            VerticesActivations[i].z = Vertices[i].Coordinats.z - ChunkPositionInWorldSpace.z;
            VerticesActivations[i].w = Vertices[i].Activation;
        }
    }

    public void GenerateChunkMesh(ComputeCubes computeCubes)
    {

        for (int i = 0; i < Vertices.Length; i++)
        {
            if (Vertices[i] == null) 
            {
                Debug.Log(i);

                continue;
            }
        }
        // _convertVerticesToActivationValuesList();

        // List<Vector3> vertices = computeCubes.ComputeTriangleVertices(VerticesActivations.ToList());

        // if (vertices.Count == 0) 
        // {
        //     _meshRenderer.ClearMesh();

        //     return;
        // }

        // ChunkMeshData meshData = new();

        // meshData.PushVertices(vertices.ToList());

        // _meshRenderer.RenderMesh(meshData);
    }

    public void LinkVertices(Vertex[] vertices)
    {
        Vertices = vertices;
    }

    public void AddVertexLink(Vertex vertex, int index)
    {
        // Vector3Int localVertexPos = vertex.Coordinats - ChunkPositionInWorldSpace;
        // var index = localVertexPos.z + localVertexPos.y * WorldDataSinglton.Instance.CHUNK_HEIGHT + localVertexPos.x * WorldDataSinglton.Instance.CHUNK_HEIGHT * WorldDataSinglton.Instance.CHUNK_SIZE;

        Vertices[index] = vertex;
    }
}
