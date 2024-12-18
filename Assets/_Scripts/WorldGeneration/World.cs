using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class World : MonoBehaviour
{
    private Dictionary<Vector3Int, Chunk> _chunks = new();
    private List<Vector3Int> _allVertices = new ();

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
                var chunkPositionInWorldSpace = new Vector3Int(x, 0, z) * WorldDataSinglton.Instance.CHUNK_SIZE;
                var chunk = Instantiate(_chunkPrefab, chunkPositionInWorldSpace, Quaternion.identity);

                chunk.GenerateChunkVoxels(chunkPositionInWorldSpace);

                _chunks.Add(chunkPositionInWorldSpace, chunk);
                _allVertices.AddRange(chunk.Vertices);
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

    public List<Vector3Int> GetVerticesInRadius(Vector3 circleCenter, float circleRadius)
    {
        return  _selectVertices.SelectVerticesByCondition(_allVertices, circleCenter, circleRadius).ToList();
    }

    public List<Vector3Int> GetVerticesByCondition(Func<Vector3Int, bool> condition)
    {
        var result = new ConcurrentBag<Vector3Int>();
        
        Parallel.ForEach(_allVertices, vertex =>
        {
            if (condition(vertex))
            {
                result.Add(vertex);
            }
        });

        return result.ToList();
    }

    private bool _checkNeighbourChunk(Vector3Int neighbourChunkCoords, Vector3Int neighbourCoords)
    {
        if (!_chunks.ContainsKey(neighbourChunkCoords) || !_chunks[neighbourChunkCoords].IsVertexInChunk(neighbourCoords)) return false;

        var neighbourChunk = _chunks[neighbourChunkCoords];

        if (neighbourChunk.ActtivationValues[neighbourCoords - neighbourChunk.ChunkPosition] > WorldDataSinglton.Instance.ACTIVATION_THRESHOLD) 
        {
            return true;
        } 

        return false;
    }

    public void AddVerticesActivation(Dictionary<Vector3Int, List<Vector3Int>> neighbours, float value)
    {
        Dictionary<Vector3Int, List<Vector3Int>> VerticesByAffectedChunks = ChunkStaticManager.DivideVerticesByChunks(neighbours.Keys.ToList());
        
        foreach (var entity in VerticesByAffectedChunks)
        {
            var chuckCoords = entity.Key;
            var verticesInChunk = entity.Value;

            if (!_chunks.ContainsKey(chuckCoords)) continue;
            
            var chunk = _chunks[chuckCoords];

            foreach(var vertex in verticesInChunk)
            {
                float updateValue = 0;

                foreach (var neighbour in neighbours[vertex])
                {
                    if (!chunk.IsVertexInChunk(neighbour))
                    {
                        var neighbourChunkCoords = ChunkStaticManager.GetChunksContainingVertex(neighbour);

                        foreach (var coords in neighbourChunkCoords)
                        {
                            if (_checkNeighbourChunk(coords, neighbour))
                            {
                                updateValue = value;   

                                break;
                            }
                        }

                        continue;
                    }

                    if (_checkNeighbourChunk(chunk.ChunkPosition, neighbour))
                    {
                        updateValue = value;   

                        break;
                    }
                }

                if (updateValue == 0) continue;

                chunk.UpdateActivationValueViaGlobalVertexPosition(vertex, updateValue);
            }
        }
        
        _renderChunksMeshesByPosition(VerticesByAffectedChunks.Keys.ToList());
    }
}
