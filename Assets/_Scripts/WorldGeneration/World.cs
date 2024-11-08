using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<Vector3Int> _vertices = new();
    private Dictionary<Vector3Int, float> _acttivationValues = new();

    [SerializeField]
    private WorldMeshRenderer _meshRenderer;

    [SerializeField]
    private ComputeCubes _computeCubes;

    private void Start()
    {
        _generateCubes();
        _generateWorldMesh();
    }

    public List<Vector3Int> GetVerticesByCondition(Func<Vector3Int, bool> condition)
    {
        return _vertices.Where((Vector3Int vertex) => condition(vertex)).ToList();
    }

    public void UpdateVerticesActivation(Dictionary<Vector3Int, float> activationValues)
    {
        foreach(var vertex in activationValues.Keys)
        {
            if (_acttivationValues.ContainsKey(vertex) && !_isOuterLayer(vertex))
            {
                _acttivationValues[vertex] = activationValues[vertex];
            }
        }

        _generateWorldMesh();
    }

    public void AddVerticesActivation(Dictionary<Vector3Int, float> activationValues)
    {
        foreach(var vertex in activationValues.Keys)
        {
            if (_acttivationValues.ContainsKey(vertex) && !_isOuterLayer(vertex))
            {
                _acttivationValues[vertex] += activationValues[vertex];

                if (_acttivationValues[vertex] > 1) _acttivationValues[vertex] = 1;
                if (_acttivationValues[vertex] < 0) _acttivationValues[vertex] = 0;
            }
        }

        _generateWorldMesh();
    }

    private void _generateWorldMesh()
    {
        List<Vector3> vertices = _computeCubes.ComputeTriangleVertices(_acttivationValues);

        if (vertices.Count == 0) return;

        ChunkMeshData meshData = new();

        meshData.PushVertices(vertices.ToList());

        _meshRenderer.RenderMesh(meshData);
    }

    private void _generateCubes()
    {
        for (int x = 0; x < WorldDataSinglton.Instance.WORLD_SIZE; x++)
        {
            for (int y = 0; y < WorldDataSinglton.Instance.WORLD_SIZE; y++)
            {
                for (int z = 0; z < WorldDataSinglton.Instance.WORLD_SIZE; z++)
                {
                    var verticePos = new Vector3Int(x, y, z);
                    _vertices.Add(verticePos);
                    var activationValue = 0f;

                    if (!_isOuterLayer(verticePos))
                        activationValue = Noise.GenerateNoiseAt((Vector3)verticePos / WorldDataSinglton.Instance.WORLD_SIZE);

                    _acttivationValues[verticePos] = activationValue;
                }
            }
        }
    }

    private bool _isOuterLayer(Vector3 vertex)
    {
        return vertex.x == 0 || vertex.x == WorldDataSinglton.Instance.WORLD_SIZE - 1 ||
                vertex.y == 0 || vertex.y == WorldDataSinglton.Instance.WORLD_SIZE - 1 ||
                vertex.z == 0 || vertex.z == WorldDataSinglton.Instance.WORLD_SIZE - 1;
    }
}
