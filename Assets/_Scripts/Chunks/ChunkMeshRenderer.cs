using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkMeshRenderer : MonoBehaviour
{
    private Mesh _mesh;
    private MeshCollider _meshCollider;
    private ChunkMeshData _meshData;
    
    private void Awake()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        _meshCollider = GetComponent<MeshCollider>();
    }

    public void SetMeshData(ChunkMeshData meshData)
    {
        _meshData = meshData;
    }

    public void RenderMesh()
    {
        _renderMesh(_meshData);
    }

    public void RenderMesh(ChunkMeshData meshData)
    {
        SetMeshData(meshData);
        _renderMesh(meshData);
    }
    
    public void ClearMesh()
    {
        _mesh.Clear();
    }

    private void _renderMesh(ChunkMeshData meshData)
    {
        _mesh.Clear();

        _mesh.vertices = meshData.Vertices.ToArray();
        _mesh.triangles = meshData.Triangles.ToArray();

        _mesh.RecalculateNormals();

        _meshCollider.sharedMesh = null;

        _meshCollider.sharedMesh = _mesh;
    }
}
