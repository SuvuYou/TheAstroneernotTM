using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class BaseMeshRenderer : MonoBehaviour
{
    private Mesh _mesh;
    private MeshCollider _meshCollider;
    private MeshData _meshData;
    
    private void Awake()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        _meshCollider = GetComponent<MeshCollider>();
    }

    public void SetMeshData(MeshData meshData)
    {
        _meshData = meshData;
    }

    public void RenderMesh()
    {
        _renderMesh(_meshData);
    }

    public void RenderMesh(MeshData meshData)
    {
        SetMeshData(meshData);
        _renderMesh(meshData);
    }
    
    public void ClearMesh()
    {
        _mesh.Clear();
    }

    private void _renderMesh(MeshData meshData)
    {
        _mesh.Clear();

        _mesh.SetVertices(meshData.GetVertices().GetActiveArraySegment().Array, 0, meshData.GetVertices().Count);
        _mesh.SetTriangles(meshData.GetTriangles().GetActiveArraySegment().Array, 0, meshData.GetTriangles().Count, 0);
        _mesh.SetUVs(0, meshData.GetUVs().GetActiveArraySegment().Array, 0, meshData.GetUVs().Count);
        
        _mesh.RecalculateNormals();

        _meshCollider.sharedMesh = null;

        if (meshData.GetVertices().Count != 0) _meshCollider.sharedMesh = _mesh;
    }
}