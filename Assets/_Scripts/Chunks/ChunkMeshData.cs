using System;
using UnityEngine;

public class ChunkMeshData
{
    private const int VERTICES_LIMIT = 20000;

    public PreallocatedArray<Vector3> Vertices = new (VERTICES_LIMIT);
    private PreallocatedArray<int> _triangles = new (VERTICES_LIMIT);

    public PreallocatedArray<int> Triangles { get => _getTriangles(Vertices.Count); }

    public ChunkMeshData() 
    { 
        _initTrianglesPreallocatedArray(VERTICES_LIMIT);
    }

    private void _initTrianglesPreallocatedArray(int verticesCount)
    {
        for (int i = 0; i < verticesCount; i++)
        {
            _triangles.Add(i);
        }
    }

    private PreallocatedArray<int> _getTriangles(int verticesCount) 
    {
        _triangles.SetCount(verticesCount);

        return _triangles;
    } 
}
