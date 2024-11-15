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
            for (int y = 0; y < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; y++)
            {
                for (int z = 0; z < WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS; z++)
                {
                    var localVerticePos = new Vector3Int(x, y, z);
                    Vertices.Add(localVerticePos);

                    var globalVerticePos = _convertLocalVertexPositionToGlobalVertexPosition(localVerticePos);
                    
                    var activationValue = 0f;

                    if (!ChunkStaticManager.IsVertexInOuterLayer(globalVerticePos))
                        activationValue = Noise.GenerateNoiseAt((Vector3)globalVerticePos / WorldDataSinglton.Instance.CHUNK_SIZE);

                    ActtivationValues[localVerticePos] = activationValue;
                }
            }
        }
    }

    private Vector3Int _convertGlobalVertexPositionToLocalVertexPosition(Vector3Int globalVertexPosition)
    {
        return globalVertexPosition - ChunkPosition;
    }

    private Vector3Int _convertLocalVertexPositionToGlobalVertexPosition(Vector3Int localVertexPosition)
    {
        return localVertexPosition + ChunkPosition;
    }
}
