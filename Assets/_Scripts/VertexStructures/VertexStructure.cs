using UnityEngine;

public abstract class VertexStructure : MonoBehaviour
{
    public Vertex[] Vertices { get; private set; }
    public Vector4[] VerticesActivations { get; private set; }

    private MeshData _meshData;

    [SerializeField]
    private BaseMeshRenderer _meshRenderer;

    public void Init(MeshData meshData, int vertexCount)
    {
        _meshData = meshData;

        Vertices  = new Vertex[vertexCount];
        VerticesActivations = new Vector4[vertexCount];
    }

    public void GenerateMesh(ComputeCubes computeCubes)
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

    public void GenerateMesh(ComputeCubesRock computeCubesRock)
    {
        _convertVerticesToActivationValuesList();

        computeCubesRock.ComputeTriangleVertices(Vertices, VerticesActivations, ref _meshData.GetVerticesRef(), ref _meshData.GetUVsRef());

        Debug.Log(_meshData.GetVertices().Count);
        
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
        var index = _convertVertexLocalPositionToArrayIndex(localPosition);

        Vertices[index] = vertex;
    }

    public void AddVertexLink(Vertex vertex, int index)
    {
        Vertices[index] = vertex;
    }

    protected abstract void _convertVerticesToActivationValuesList();

    protected abstract int _convertVertexLocalPositionToArrayIndex(Vector3Int localPosition);
} 