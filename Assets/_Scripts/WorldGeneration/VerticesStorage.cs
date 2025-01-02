using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class VerticesStorage
{
    private Dictionary<Vector3Int, Vertex> _vertices = new ();
    private ConcurrentBag<Vector3Int> _bagOfVertices = new ();

    private PreallocatedArray<Vector3Int> _selectedVerticesHolder = new (5000); 

    public void CreateChunkVertices(ChunkAlloc chunk)
    {
        for (int x = 0; x < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; x++)
        {
            for (int z = 0; z < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; z++)
            {
                var globalColumnPos = new Vector2Int(x + chunk.ChunkPositionInWorldSpace.x, z + chunk.ChunkPositionInWorldSpace.z);

                if (_vertices.ContainsKey(new Vector3Int(globalColumnPos.x, 0, globalColumnPos.y))) continue;

                var columnHeight = Noise.GenerateNoiseAtPosition(globalColumnPos, WorldDataSinglton.Instance.TERRAIN_NOISE_SETTINGS) * WorldDataSinglton.Instance.CHUNK_HEIGHT;

                for (int y = 0; y < WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS; y++)
                {   
                    var localVertexPos = new Vector3Int(x, y, z);
                    var globalVertexPos = localVertexPos + chunk.ChunkPositionInWorldSpace;
                    var avtivationValue = _selectActivationValue(localVertexPos, globalVertexPos, columnHeight);

                    _createVertexAtPosition(globalVertexPos, avtivationValue);
                }
            }
        }
    }

    private float _selectActivationValue(Vector3Int localVertexPos, Vector3Int globalVertexPos, float columnHeight)
    {
        if (localVertexPos.y <= 1)
            return 1f; 
        else if (localVertexPos.y > columnHeight)
            return 0f;
        else if (localVertexPos.y > WorldDataSinglton.Instance.CAVE_THRESHOLD && localVertexPos.y <= columnHeight)
            return 1f; 
        else
            return Noise.GenerateNoiseAtPosition(globalVertexPos, WorldDataSinglton.Instance.CAVE_NOISE_SETTINGS);
    }

    private void _createVertexAtPosition(Vector3Int globalVertexPos, float activationValue)
    {
        // Check if vertex is gooning and an the edge
        var isEdgeVertex = ChunkStaticManagerAlloc.IsVertexIntersectingChunksOnXAxis(globalVertexPos) || ChunkStaticManagerAlloc.IsVertexIntersectingChunksOnZAxis(globalVertexPos);
        
        _vertices.Add(globalVertexPos, new Vertex(globalVertexPos, activationValue, isEdgeVertex));
    }

    public void LinkVerticesToChunks(ChunkAlloc chunk)
    {
        for (int x = 0; x < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; x++)
        {
            for (int y = 0; y < WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS; y++)
            {   
                for (int z = 0; z < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; z++)
                {
                    var localVertexPos = new Vector3Int(x, y, z);
                    var globalVertexPos = localVertexPos + chunk.ChunkPositionInWorldSpace;

                    _vertices[globalVertexPos].AddParentChunkLink(chunk);
                    chunk.AddVertexLink(_vertices[globalVertexPos], localVertexPos);
                }
            }
        }
    }

    public void SelectVerticesByConditionInBounds(Func<Vector3Int, bool> condition, Vector3Int lowerBounds, Vector3Int upperBounds)
    {
        _bagOfVertices.Clear();

        Parallel.For(lowerBounds.x, upperBounds.x + 1, x =>
        {
            for (int y = lowerBounds.y; y <= upperBounds.y; y++)
            {
                for (int z = lowerBounds.z; z <= upperBounds.z; z++)
                {
                    var vertex = new Vector3Int(x, y, z);

                    if (condition(vertex))
                    {
                        _bagOfVertices.Add(vertex);
                    }
                }
            }
        });
        
        _selectedVerticesHolder.ResetCount();

        foreach(var item in _bagOfVertices)
        {
            if (_vertices.ContainsKey(item)) _selectedVerticesHolder.AddWithResize(item);
        }
    }

    public void AddActivationToSelectedVertices(float activationIncrement)
    {
        Parallel.For(0, _selectedVerticesHolder.Count, i =>
        {
            var vertexCoords = _selectedVerticesHolder.FullArray[i];
            _vertices[vertexCoords].AddActivation(activationIncrement);
        });
    }

    public Vector3Int[] GetChunksOfSelectedVertices()
    {
        return ChunkStaticManagerAlloc.GetChunksContainingVertices(_selectedVerticesHolder);
    }
}