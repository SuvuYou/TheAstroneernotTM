using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class World : MonoBehaviour
{
    private Dictionary<Vector3Int, Chunk> _chunks = new();
    private Dictionary<Vector3Int, Vertex> _vertices = new();

    [SerializeField]
    private Chunk _chunkPrefab;

    [SerializeField]
    private ComputeCubes _computeCubes;

    [SerializeField]
    private SelectVertices _selectVertices;

    private float _cachedActivationValue = 0;

    private void Start()
    {
        _initChunks();
        _renderAllChunksMeshes();
    }

    private void Update()
    {
        if (_cachedActivationValue != WorldDataSinglton.Instance.ACTIVATION_THRESHOLD)
            _renderAllChunksMeshes();
    }

    public void Regenerate()
    {
        _clearChunks();
        _initChunks();
        _renderAllChunksMeshes();
    }

    private void _initChunks()
    {
        for (int x = 0; x < WorldDataSinglton.Instance.RENDER_DISTANCE; x += 1)
        {
            for (int z = 0; z < WorldDataSinglton.Instance.RENDER_DISTANCE; z += 1)
            {
                var chunk = _createChunk(x, z);
                _createChunkVertices(chunk); 
                _linkVerticesToChunks(chunk);
            }
        }
    }

    private Chunk _createChunk(int chunkPositionX, int chunkPositionZ)
    {
        var chunkPositionInWorldSpace = new Vector3Int(chunkPositionX, 0, chunkPositionZ) * WorldDataSinglton.Instance.CHUNK_SIZE;
        var chunk = Instantiate(_chunkPrefab, chunkPositionInWorldSpace, Quaternion.identity);

        chunk.Init(chunkPositionInWorldSpace);

        _chunks.Add(chunkPositionInWorldSpace, chunk);

        return chunk;
    }

    private void _createChunkVertices(Chunk chunk)
    {
        for (int x = 0; x < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; x++)
        {
            for (int z = 0; z < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; z++)
            {
                var localColumnPos = new Vector2Int(x, z);
                var globalColumnPos = localColumnPos + new Vector2Int(chunk.ChunkPositionInWorldSpace.x, chunk.ChunkPositionInWorldSpace.z);

                if (_vertices.ContainsKey(new Vector3Int(globalColumnPos.x, 0, globalColumnPos.y))) continue;


                var columnHeight = Noise.GenerateNoiseAtPosition(globalColumnPos, WorldDataSinglton.Instance.TERRAIN_NOISE_SETTINGS) * WorldDataSinglton.Instance.CHUNK_HEIGHT;

                for (int y = WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS - 1; y >= 0; y--)
                {   
                    var localVertexPos = new Vector3Int(x, y, z);
                    var globalVertexPos = localVertexPos + chunk.ChunkPositionInWorldSpace;
                    
                    var activationValue = 0f;

                    if (!ChunkStaticManager.IsVertexInOuterLayer(globalVertexPos))
                    {
                        if (y <= 1)
                            activationValue = 1f; 
                        else if (y > columnHeight)
                            activationValue = 0f; 
                        else if (y > WorldDataSinglton.Instance.CAVE_THRESHOLD && y <= columnHeight)
                            activationValue = 1f; 
                        else
                            activationValue = Noise.GenerateNoiseAtPosition(globalVertexPos, WorldDataSinglton.Instance.CAVE_NOISE_SETTINGS);
                    }

                    // Check if vertex is gooning and an the edge
                    var isEdgeVertex = x == 0 || x == WorldDataSinglton.Instance.CHUNK_SIZE || z == 0 || z == WorldDataSinglton.Instance.CHUNK_SIZE;
                    
                    _vertices.Add(globalVertexPos, new Vertex(globalVertexPos, activationValue, isEdgeVertex));
                }
            }
        }
    }

    private void _linkVerticesToChunks(Chunk chunk)
    {
        for (int x = 0; x < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; x++)
        {
            for (int z = 0; z < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; z++)
            {
                for (int y = WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS - 1; y >= 0; y--)
                {   
                    var localVertexPos = new Vector3Int(x, y, z);
                    var globalVertexPos = localVertexPos + chunk.ChunkPositionInWorldSpace;

                    _vertices[globalVertexPos].AddParentChunkLink(chunk);
                    chunk.AddVertexLink(_vertices[globalVertexPos]);
                }
            }
        }
    }

    private void _clearChunks()
    {
        foreach (var chunk in _chunks.Values)
            Destroy(chunk.gameObject);
        
        _chunks.Clear();
    }

    private void _renderAllChunksMeshes() => _renderChunksMeshes(_chunks.Values.ToList());
    
    private void _renderChunksMeshesByPosition(List<Vector3Int> chunks) => _renderChunksMeshes(_chunks.Where(coordinats => chunks.Contains(coordinats.Key)).Select(chunks => chunks.Value).ToList());

    private void _renderChunksMeshes(List<Chunk> chunks)
    {
        _cachedActivationValue = WorldDataSinglton.Instance.ACTIVATION_THRESHOLD;

        foreach (var chunk in chunks)
        {
            chunk.GenerateChunkMesh(_computeCubes);
        }
    }

    public List<Vector3Int> GetVerticesInRadius(Vector3 circleCenter, float circleRadius)
    {
        return _selectVertices.SelectVerticesByCondition(_vertices.Keys.ToList(), circleCenter, circleRadius).ToList();
    }

    public List<Vector3Int> GetVerticesByCondition(Func<Vector3Int, bool> condition)
    {
        var result = new ConcurrentBag<Vector3Int>();
        Parallel.ForEach(_vertices.Keys, vertex =>
        {
            if (condition(vertex))
            {
                result.Add(vertex);
            }
        });

        return result.ToList();
    }

    public void AddVerticesActivation(List<Vector3Int> vertices, float value)
    {
        Parallel.ForEach(vertices, vertexCoords =>
        {
            if (_vertices.ContainsKey(vertexCoords)) _vertices[vertexCoords].AddActivation(activationIncrement: value);
        });
        
        List<Vector3Int> affectedChunks = ChunkStaticManager.GetChunksContainingVertices(vertices);

        _renderChunksMeshesByPosition(affectedChunks);
    }
}
