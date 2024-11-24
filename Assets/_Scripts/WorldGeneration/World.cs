using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{
    private Dictionary<Vector3Int, Chunk> _chunks = new();

    [SerializeField]
    private Chunk _chunkPrefab;

    [SerializeField]
    private ComputeCubes _computeCubes;

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
                var chunkPosition = new Vector3Int(x, 0, z) * WorldDataSinglton.Instance.CHUNK_SIZE;
                var chunk = Instantiate(_chunkPrefab, chunkPosition, Quaternion.identity);
                chunk.GenerateChunkVoxels(chunkPosition);

                _chunks.Add(chunkPosition, chunk);
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
    
    private void _renderChunksMeshesByPosition(List<Vector3Int> chunks) => _renderChunksMeshes(_chunks.Where(pair => chunks.Contains(pair.Key)).Select(pair => pair.Value).ToList());

    private void _renderChunksMeshes(List<Chunk> chunks)
    {
        _cachedActivationValue = WorldDataSinglton.Instance.ACTIVATION_THRESHOLD;

        foreach (var chunk in chunks)
        {
            chunk.GenerateChunkMesh(_computeCubes);
        }
    }

    public List<Vector3Int> GetVerticesByCondition(Func<Vector3Int, bool> condition)
    {
        return _chunks.Values.SelectMany(chunk => chunk.Vertices.Select(vertex => vertex + chunk.ChunkPosition)).Where((Vector3Int vertex) => condition(vertex)).ToList();
    }

    public void AddVerticesActivation(Dictionary<Vector3Int, float> activationValues)
    {
        Dictionary<Vector3Int, List<Vector3Int>> VerticesByAffectedChunks = ChunkStaticManager.DivideVerticesByChunks(activationValues.Keys.ToList());
        
        foreach (var entity in VerticesByAffectedChunks)
        {
            if (!_chunks.ContainsKey(entity.Key)) continue;
            
            var chunk = _chunks[entity.Key];

            foreach(var vertex in entity.Value)
            {
                chunk.UpdateActivationValueViaGlobalVertexPosition(vertex, activationValues[vertex]);
            }
        }
        
        _renderChunksMeshesByPosition(VerticesByAffectedChunks.Keys.ToList());
    }
}
