using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3Int ChunkPosition { get; private set; }

    public List<Vector3Int> Vertices { get; private set; } = new();
    public Dictionary<Vector3Int, float> ActtivationValues { get; private set; } = new();

    [SerializeField]
    private ChunkMeshRenderer _meshRenderer;

    public void UpdateActivationValueViaGlobalVertexPosition(Vector3Int gloalVertexPosition, float activationValue) 
    {
        if (ChunkStaticManager.IsVertexInOuterLayer(gloalVertexPosition) || gloalVertexPosition.y <= 1) return;

        var localVertexPosition = _convertGlobalVertexPositionToLocalVertexPosition(gloalVertexPosition);

        ActtivationValues[localVertexPosition] += activationValue;

        if (ActtivationValues[localVertexPosition] > 1) ActtivationValues[localVertexPosition] = 1;
        if (ActtivationValues[localVertexPosition] < 0) ActtivationValues[localVertexPosition] = 0;
    }

    public void GenerateChunkMesh(ComputeCubes computeCubes)
    {
        List<Vector3> vertices = computeCubes.ComputeTriangleVertices(ActtivationValues);

        if (vertices.Count == 0) 
        {
            _meshRenderer.ClearMesh();

            return;
        }

        ChunkMeshData meshData = new();

        meshData.PushVertices(vertices.ToList());

        _meshRenderer.RenderMesh(meshData);
    }

    public void GenerateChunkVoxels(Vector3Int chunkPosition)
    {
        ChunkPosition = chunkPosition;

        for (int x = 0; x < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; x++)
        {
            for (int z = 0; z < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; z++)
            {
                var localColumnPos = new Vector2Int(x, z);
                var globalColumnPos = _convertLocalVertexPositionToGlobalVertexPosition(localColumnPos);
                var columnHeight = Noise.GenerateNoiseAtPosition(globalColumnPos, WorldDataSinglton.Instance.TERRAIN_NOISE_SETTINGS) * WorldDataSinglton.Instance.CHUNK_HEIGHT;

                for (int y = WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS - 1; y >= 0; y--)
                {   
                    var localVerticePos = new Vector3Int(x, y, z);
                    var globalVerticePos = _convertLocalVertexPositionToGlobalVertexPosition(localVerticePos);

                    Vertices.Add(globalVerticePos);
                    
                    var activationValue = 0f;

                    if (!ChunkStaticManager.IsVertexInOuterLayer(globalVerticePos))
                    {
                        if (y <= 1)
                            activationValue = 1f; 
                        else if (y > columnHeight)
                            activationValue = 0f; 
                        else if (y > WorldDataSinglton.Instance.CAVE_THRESHOLD && y <= columnHeight)
                            activationValue = 1f; 
                        else
                            activationValue = Noise.GenerateNoiseAtPosition(globalVerticePos, WorldDataSinglton.Instance.CAVE_NOISE_SETTINGS);
                    }

                    ActtivationValues[localVerticePos] = activationValue;
                }
            }
        }
    }

    public bool IsVertexInChunk(Vector3Int vertex)
    {
        return vertex.x >= ChunkPosition.x && vertex.x < ChunkPosition.x + WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS && vertex.z >= ChunkPosition.z && vertex.z < ChunkPosition.z + WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS && vertex.y >= 0 && vertex.y < WorldDataSinglton.Instance.CHUNK_HEIGHT;
    }

    private Vector3Int _convertGlobalVertexPositionToLocalVertexPosition(Vector3Int globalVertexPosition)
    {
        return globalVertexPosition - ChunkPosition;
    }

    private Vector3Int _convertLocalVertexPositionToGlobalVertexPosition(Vector3Int localVertexPosition)
    {
        return localVertexPosition + ChunkPosition;
    }

    private Vector2Int _convertLocalVertexPositionToGlobalVertexPosition(Vector2Int localVertexPosition)
    {
        return localVertexPosition + new Vector2Int(ChunkPosition.x, ChunkPosition.z);
    }
}
