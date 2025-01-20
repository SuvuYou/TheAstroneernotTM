using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{
    private Dictionary<Vector3Int, ChunkAlloc> _chunks = new();

    [SerializeField]
    private ChunkAlloc _chunkPrefab;

    [SerializeField]
    private ComputeCubes _computeCubes;

    [SerializeField]
    private VertexTypeSelector _vertexTypeSelector;

    private float _cachedActivationValue = 0;

    private VerticesStorage _verticesStorage = new();

    private void Start()
    {
        _verticesStorage.InitVertexTypeSelector(_vertexTypeSelector);

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
                _verticesStorage.CreateChunkVertices(chunk);
                _verticesStorage.LinkVerticesToChunks(chunk);
            }
        }
    }

    private ChunkAlloc _createChunk(int chunkPositionX, int chunkPositionZ)
    {
        var chunkPositionInWorldSpace = new Vector3Int(chunkPositionX, 0, chunkPositionZ) * WorldDataSinglton.Instance.CHUNK_SIZE;
        var chunk = Instantiate(_chunkPrefab, chunkPositionInWorldSpace, Quaternion.identity);

        chunk.Init(chunkPositionInWorldSpace);

        _chunks.Add(chunkPositionInWorldSpace, chunk);

        return chunk;
    }

    private void _clearChunks()
    {
        foreach (var chunk in _chunks.Values)
            Destroy(chunk.gameObject);
        
        _chunks.Clear();
    }

    private void _renderAllChunksMeshes() => _renderChunksMeshes(_chunks.Values.ToList());
    
    private void _renderChunksMeshesByPosition(Vector3Int[] chunks) => _renderChunksMeshes(_chunks.Where(coordinats => chunks.Contains(coordinats.Key)).Select(chunks => chunks.Value).ToList());

    private void _renderChunksMeshes(List<ChunkAlloc> chunks)
    {
        _cachedActivationValue = WorldDataSinglton.Instance.ACTIVATION_THRESHOLD;

        foreach (var chunk in chunks)
        {
            chunk.GenerateChunkMesh(_computeCubes);
        }
    }

    public void AddActivationToVerticesByCondition(Func<Vector3Int, bool> condition, Func<Vector3Int, float> percentageOfRadious, Vector3Int lowerBounds, Vector3Int upperBounds, float activationValueIncrement)
    {
        _verticesStorage.SelectVerticesByConditionInBounds(condition, percentageOfRadious, lowerBounds, upperBounds);
        _verticesStorage.AddActivationToSelectedVertices(activationValueIncrement);
        _renderChunksMeshesByPosition(_verticesStorage.GetChunksOfSelectedVertices());
    }
}