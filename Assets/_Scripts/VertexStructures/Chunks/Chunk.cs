using UnityEngine;

public class Chunk : VertexStructure
{
    public Vector3Int ChunkPositionInWorldSpace { get; private set; }

    public void Init(Vector3Int chunkPositionInWorldSpace) 
    {
        base.Init(
            meshData: new ChunkMeshData(), 
            vertexCount: WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS * WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS
        );
        
        ChunkPositionInWorldSpace = chunkPositionInWorldSpace;
    }     

    protected override void _convertVerticesToActivationValuesList()
    {
        for (int i = 0; i < Vertices.Length; i++)
        {
            VerticesActivations[i].x = Vertices[i].Coordinats.x - ChunkPositionInWorldSpace.x;
            VerticesActivations[i].y = Vertices[i].Coordinats.y - ChunkPositionInWorldSpace.y;
            VerticesActivations[i].z = Vertices[i].Coordinats.z - ChunkPositionInWorldSpace.z;
            VerticesActivations[i].w = Vertices[i].Activation;
        }
    }

    protected override int _convertVertexLocalPositionToArrayIndex(Vector3Int localPosition)
    {
        // WARNING: Needs to be the same as in MarchingCubes.compute indexFromCoord function
        return localPosition.x * WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS + localPosition.z * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS + localPosition.y;
    }
}
