using UnityEngine;

public static class ChunkStaticManagerAlloc
{
    private static Vector3Int[] resultChunkPositions = new Vector3Int[10000];

    private static Vector3Int _originalChunkPositionHolder = Vector3Int.zero;
    private static Vector3Int[] _chunksHolder = new Vector3Int[4];

    public static Vector3Int[] GetChunksContainingVertices(Vector3Int[] vertices, int Length)
    {
        int count = 0;

        for (int i = 0; i < Length; i++)
        {
            int chunksCount = GetChunksContainingVertex(vertices[i]);

            for (int j = 0; j < chunksCount; j++)
            {
                var chunk = _chunksHolder[j];

                bool isDuplicate = false;

                for (int k = 0; k < count; k++)
                {
                    if (resultChunkPositions[k] == chunk)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    resultChunkPositions[count++] = chunk;
                }

            }
        }

        // Return the result with exact number of chunks
        Vector3Int[] result = new Vector3Int[count];
        System.Array.Copy(resultChunkPositions, result, count);

        return result;
    }

    public static int GetChunksContainingVertex(Vector3Int globalVertexPosition)
    {
        int count = 0;

        GetChunkCoordinatsByGlobalVertexPosition(globalVertexPosition);
        _chunksHolder[count++] = _originalChunkPositionHolder;

        bool xIntersection = _isVertexIntersectingChunksOnXAxis(globalVertexPosition);
        bool zIntersection = _isVertexIntersectingChunksOnZAxis(globalVertexPosition);

        if (xIntersection)
        {
            _chunksHolder[count++] = _originalChunkPositionHolder - new Vector3Int(WorldDataSinglton.Instance.CHUNK_SIZE, 0, 0);
        }

        if (zIntersection)
        {
            _chunksHolder[count++] = _originalChunkPositionHolder - new Vector3Int(0, 0, WorldDataSinglton.Instance.CHUNK_SIZE);
        }

        if (xIntersection && zIntersection)
        {
            _chunksHolder[count++] = _originalChunkPositionHolder - new Vector3Int(WorldDataSinglton.Instance.CHUNK_SIZE, 0, WorldDataSinglton.Instance.CHUNK_SIZE);
        }

        return count;
    }

    public static void GetChunkCoordinatsByGlobalVertexPosition(Vector3Int globalVertexPosition)
    {
        int x = globalVertexPosition.x / WorldDataSinglton.Instance.CHUNK_SIZE * WorldDataSinglton.Instance.CHUNK_SIZE;
        int z = globalVertexPosition.z / WorldDataSinglton.Instance.CHUNK_SIZE * WorldDataSinglton.Instance.CHUNK_SIZE;

        _originalChunkPositionHolder.x = x;
        _originalChunkPositionHolder.z = z;
    }

    private static bool _isVertexIntersectingChunksOnXAxis(Vector3 globalVertexPosition)
    {
        return globalVertexPosition.x % WorldDataSinglton.Instance.CHUNK_SIZE == 0;
    }

    private static bool _isVertexIntersectingChunksOnZAxis(Vector3 globalVertexPosition)
    {
        return globalVertexPosition.z % WorldDataSinglton.Instance.CHUNK_SIZE == 0;
    }

    // Custom data structure to hold chunk and vertex pairs
    public struct ChunkVertexPair
    {
        public Vector3Int ChunkPosition;
        public Vector3Int[] Vertices;
        private int _vertexCount;

        public ChunkVertexPair(Vector3Int chunkPosition, Vector3Int firstVertex)
        {
            ChunkPosition = chunkPosition;
            Vertices = new Vector3Int[4]; // Max 4 vertices per chunk
            _vertexCount = 0;
            AddVertex(firstVertex);
        }

        public void AddVertex(Vector3Int vertex)
        {
            if (_vertexCount < Vertices.Length)
            {
                Vertices[_vertexCount++] = vertex;
            }
        }
    }
}