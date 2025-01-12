using UnityEngine;

public class ChunkAlloc : MonoBehaviour
{
    public Vector3Int ChunkPositionInWorldSpace { get; private set; }

    public Vertex[] Vertices { get; private set; }
    public Vector4[] VerticesActivations { get; private set; }

    private ChunkMeshData _meshData = new();

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
            VerticesActivations[i].x = Vertices[i].Coordinats.x - ChunkPositionInWorldSpace.x;
            VerticesActivations[i].y = Vertices[i].Coordinats.y - ChunkPositionInWorldSpace.y;
            VerticesActivations[i].z = Vertices[i].Coordinats.z - ChunkPositionInWorldSpace.z;
            VerticesActivations[i].w = Vertices[i].Activation;
        }
    }

    public void GenerateChunkMesh(ComputeCubes computeCubes)
    {
        _convertVerticesToActivationValuesList();

        computeCubes.ComputeTriangleVertices(Vertices, VerticesActivations, ref _meshData.GetVerticesRef(), ref _meshData.GetUVsRef());

        if (_meshData.GetVertices().Count == 0) 
        {
            _meshRenderer.ClearMesh();
            _meshData.Clear();

            return;
        }

        _meshRenderer.RenderMesh(_meshData);
        _meshData.Clear();
    }

    public void AddVertexLink(Vertex vertex, Vector3Int localPosition)
    {
        // WARNING: Needs to be the same as in MarchingCubes.compute indexFromCoord function
        var index = localPosition.x * WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS + localPosition.z * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS + localPosition.y;

        Vertices[index] = vertex;
    }
}
