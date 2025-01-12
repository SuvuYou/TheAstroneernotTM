using UnityEngine;

public class ChunkMeshData
{
    private const int INIT_VERTICES_LIMIT = 12000;

    private PreallocatedArray<Vector3> _vertices = new (INIT_VERTICES_LIMIT);
    private PreallocatedArray<int> _triangles = new (INIT_VERTICES_LIMIT);
    private PreallocatedArray<Vector2> _UVs = new (INIT_VERTICES_LIMIT);

    public PreallocatedArray<Vector3> GetVertices() => _vertices;
    public ref PreallocatedArray<Vector3> GetVerticesRef() => ref _vertices;

    public PreallocatedArray<int> GetTriangles() 
    {
        _triangles.SetCount(_vertices.Count);

        return _triangles;
    }

    public PreallocatedArray<Vector2> GetUVs() => _UVs;
    public ref PreallocatedArray<Vector2> GetUVsRef() => ref _UVs;

    public ChunkMeshData() 
    { 
        _initTrianglesPreallocatedArray(INIT_VERTICES_LIMIT);
    }

    public void Clear() 
    {
        _vertices.ResetCount();
        _triangles.ResetCount();
        _UVs.ResetCount();
    }

    private void _initTrianglesPreallocatedArray(int verticesCount)
    {
        for (int i = 0; i < verticesCount; i++)
        {
            _triangles.AddWithResize(i);
        }
    }
}
