using UnityEngine;

public abstract class MeshData
{
    protected int INIT_VERTICES_LIMIT;

    private PreallocatedArray<Vector3> _vertices;
    private PreallocatedArray<int> _triangles;
    private PreallocatedArray<Vector2> _UVs;

    public PreallocatedArray<Vector3> GetVertices() => _vertices;
    public ref PreallocatedArray<Vector3> GetVerticesRef() => ref _vertices;

    public PreallocatedArray<int> GetTriangles()
    {
        _triangles.SetCount(_vertices.Count);
        return _triangles;
    }

    public PreallocatedArray<Vector2> GetUVs() => _UVs;
    public ref PreallocatedArray<Vector2> GetUVsRef() => ref _UVs;

    protected MeshData(int verticesLimit)
    {
        INIT_VERTICES_LIMIT = verticesLimit;

        _vertices = new PreallocatedArray<Vector3>(INIT_VERTICES_LIMIT);
        _triangles = new PreallocatedArray<int>(INIT_VERTICES_LIMIT);
        _UVs = new PreallocatedArray<Vector2>(INIT_VERTICES_LIMIT);

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