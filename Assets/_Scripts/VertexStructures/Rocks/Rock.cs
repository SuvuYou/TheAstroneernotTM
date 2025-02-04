using UnityEngine;

public class Rock : VertexStructure
{
    public void Init() 
    {
        base.Init(
            meshData: new RockMeshData(), 
            vertexCount: (int)Mathf.Pow(WorldDataSinglton.Instance.ROCK_SIZE, 3)
        );
    }     

    protected override void _convertVerticesToActivationValuesList()
    {
        for (int i = 0; i < Vertices.Length; i++)
        {
            VerticesActivations[i].x = Vertices[i].Coordinats.x;
            VerticesActivations[i].y = Vertices[i].Coordinats.y;
            VerticesActivations[i].z = Vertices[i].Coordinats.z;
            VerticesActivations[i].w = Vertices[i].Activation;
        }
    }

    protected override int _convertVertexLocalPositionToArrayIndex(Vector3Int localPosition)
    {
        return localPosition.x * WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS + localPosition.z * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS + localPosition.y;
    }
}

