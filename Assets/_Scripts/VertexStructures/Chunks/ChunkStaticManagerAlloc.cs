using UnityEngine;

public static class ChunkStaticManagerAlloc
{
    private static PreallocatedArray<Vector3Int> _resultChunkPositions = new (1000);

    private static Vector3Int _originalChunkPositionHolder = Vector3Int.zero;
    private static PreallocatedArray<Vector3Int> _chunksHolder = new (4);

    public static Vector3Int[] GetChunksContainingVertices(PreallocatedArray<VertexActivationHolder> vertices)
    {
        _resultChunkPositions.ResetCount();

        for (int i = 0; i < vertices.Count; i++)
        {
            _collectChunksContainingVertex(vertices.FullArray[i].vertex);

            for (int j = 0; j < _chunksHolder.Count; j++)
            {
                var chunk = _chunksHolder.FullArray[j];

                bool isDuplicate = _resultChunkPositions.Contains(chunk);

                if (!isDuplicate)
                {
                    _resultChunkPositions.AddWithResize(chunk);
                }
            }
        }

        return _resultChunkPositions.GetActiveArraySegment().ToArray();
    }

    private static void _collectChunksContainingVertex(Vector3Int globalVertexPosition)
    {
        _chunksHolder.ResetCount();

        GetChunkCoordinatsByGlobalVertexPosition(globalVertexPosition);
        _chunksHolder.Add(_originalChunkPositionHolder);

        bool xIntersection = IsVertexIntersectingChunksOnXAxis(globalVertexPosition);
        bool zIntersection = IsVertexIntersectingChunksOnZAxis(globalVertexPosition);

        if (xIntersection)
        {
            _chunksHolder.Add(_originalChunkPositionHolder - new Vector3Int(WorldDataSinglton.Instance.CHUNK_SIZE, 0, 0));
        }

        if (zIntersection)
        {
            _chunksHolder.Add(_originalChunkPositionHolder - new Vector3Int(0, 0, WorldDataSinglton.Instance.CHUNK_SIZE));
        }

        if (xIntersection && zIntersection)
        {
            _chunksHolder.Add(_originalChunkPositionHolder - new Vector3Int(WorldDataSinglton.Instance.CHUNK_SIZE, 0, WorldDataSinglton.Instance.CHUNK_SIZE));
        }
    }

    public static void GetChunkCoordinatsByGlobalVertexPosition(Vector3Int globalVertexPosition)
    {
        int x = globalVertexPosition.x / WorldDataSinglton.Instance.CHUNK_SIZE * WorldDataSinglton.Instance.CHUNK_SIZE;
        int z = globalVertexPosition.z / WorldDataSinglton.Instance.CHUNK_SIZE * WorldDataSinglton.Instance.CHUNK_SIZE;

        _originalChunkPositionHolder.x = x;
        _originalChunkPositionHolder.z = z;
    }

    public static bool IsVertexIntersectingChunksOnXAxis(Vector3 globalVertexPosition)
    {
        return globalVertexPosition.x % WorldDataSinglton.Instance.CHUNK_SIZE == 0;
    }

    public static bool IsVertexIntersectingChunksOnZAxis(Vector3 globalVertexPosition)
    {
        return globalVertexPosition.z % WorldDataSinglton.Instance.CHUNK_SIZE == 0;
    }
}