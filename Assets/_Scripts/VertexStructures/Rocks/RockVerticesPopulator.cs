using System.Collections.Generic;
using UnityEngine;

public class RockVerticesPopulator
{
    private Dictionary<Vector3, Vertex> _vertices = new ();

    public void CreateRockVertices()
    {
        for (float x = 0; x < WorldDataSinglton.Instance.ROCK_SIZE; x++)
        {
            for (float z = 0; z < WorldDataSinglton.Instance.ROCK_SIZE; z++)
            {
                for (float y = 0; y < WorldDataSinglton.Instance.ROCK_SIZE; y++)       
                {
                    var localVertexPos = new Vector3(x * 1, y * 1, z * 1);
                    var avtivationValue = 1;

                    if (x == 0 || z == 0 || y == 0 || x > WorldDataSinglton.Instance.ROCK_SIZE - 3 || z > WorldDataSinglton.Instance.ROCK_SIZE - 3 || y > WorldDataSinglton.Instance.ROCK_SIZE - 3)
                        avtivationValue = 0;

                    _createVertexAtPosition(localVertexPos, VertexType.Bedrock, avtivationValue);
                }
            }
        }
    }

    private void _createVertexAtPosition(Vector3 globalVertexPos, VertexType vertexType, float activationValue)
    {
        _vertices.Add(globalVertexPos, new Vertex(globalVertexPos, vertexType, activationValue, isEdgeVertex: false));
    }

    public void LinkVerticesToRock(Rock rock)
    {
        int index = 0;

        for (float x = 0; x < WorldDataSinglton.Instance.ROCK_SIZE; x++)
        {
            for (float z = 0; z < WorldDataSinglton.Instance.ROCK_SIZE; z++) 
            {
                for (float y = 0; y < WorldDataSinglton.Instance.ROCK_SIZE; y++)   
                {
                    var localVertexPos = new Vector3(x * 1, y * 1, z * 1);

                    rock.AddVertexLink(_vertices[localVertexPos], index);

                    index++;
                }
            }
        }
    }
}