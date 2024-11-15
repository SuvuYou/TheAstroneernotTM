using System.Collections.Generic;
using UnityEngine;

public static class ChunkStaticManager
{
    public static Dictionary<Vector3Int, List<Vector3Int>> DivideVerticesByChunks(List<Vector3Int> vertices)
    {
        Dictionary<Vector3Int, List<Vector3Int>> chunksWithVertices = new ();

        foreach (var vertex in vertices)
        {
            var chunksPositions = _getChunksContainingVertex(vertex);

            foreach(var chunkPosition in chunksPositions)
            {
                if (!chunksWithVertices.ContainsKey(chunkPosition))
                {
                    chunksWithVertices.Add(chunkPosition, new List<Vector3Int>());
                }

                chunksWithVertices[chunkPosition].Add(vertex);
            }
        }

        return chunksWithVertices;
    }

    private static List<Vector3Int> _getChunksContainingVertex(Vector3Int globalVertexPosition)
    {
        List<Vector3Int> chunkPositions = new ();

        var originalChunkPosition = _getLocalVertexPosition(globalVertexPosition);
        chunkPositions.Add(originalChunkPosition);

        // Up to 3 intersection chunks
        var xIntersection = _isVertexIntersectingChunksOnXAxis(globalVertexPosition);
        var zIntersection = _isVertexIntersectingChunksOnZAxis(globalVertexPosition);

        if (xIntersection)
        {
            chunkPositions.Add(originalChunkPosition - new Vector3Int(WorldDataSinglton.Instance.CHUNK_SIZE, 0, 0));
        }

        if (zIntersection)
        {
            chunkPositions.Add(originalChunkPosition - new Vector3Int(0, 0, WorldDataSinglton.Instance.CHUNK_SIZE));
        }

        if(xIntersection && zIntersection)
        {
            chunkPositions.Add(originalChunkPosition - new Vector3Int(WorldDataSinglton.Instance.CHUNK_SIZE, 0, WorldDataSinglton.Instance.CHUNK_SIZE));
        }

        return chunkPositions;
    }

    private static Vector3Int _getLocalVertexPosition(Vector3Int globalVertexPosition)
    {
        var x = globalVertexPosition.x / WorldDataSinglton.Instance.CHUNK_SIZE * WorldDataSinglton.Instance.CHUNK_SIZE;
        var z = globalVertexPosition.z / WorldDataSinglton.Instance.CHUNK_SIZE * WorldDataSinglton.Instance.CHUNK_SIZE;

        return new Vector3Int(x, 0, z);
    }

    public static bool IsVertexInOuterLayer(Vector3 vertex)
    {
        return vertex.x == 0 || vertex.x == WorldDataSinglton.Instance.RENDER_DISTANCE * WorldDataSinglton.Instance.CHUNK_SIZE ||
                vertex.y == 0 || vertex.y == WorldDataSinglton.Instance.RENDER_DISTANCE * WorldDataSinglton.Instance.CHUNK_SIZE ||
                vertex.z == 0 || vertex.z == WorldDataSinglton.Instance.RENDER_DISTANCE * WorldDataSinglton.Instance.CHUNK_SIZE;
    }

    private static bool _isVertexIntersectingChunksOnXAxis(Vector3 globalVertexPosition)
    {
        return globalVertexPosition.x % WorldDataSinglton.Instance.CHUNK_SIZE == 0;
    }

    private static bool _isVertexIntersectingChunksOnZAxis(Vector3 globalVertexPosition)
    {
        return globalVertexPosition.z % WorldDataSinglton.Instance.CHUNK_SIZE == 0;
    }
}
