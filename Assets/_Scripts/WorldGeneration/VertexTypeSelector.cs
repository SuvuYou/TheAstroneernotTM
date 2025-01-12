using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VertexTypeSelector", menuName = "ScriptableObjects/VertexTypeSelector")]
public class VertexTypeSelector : ScriptableObject
{
    public SerializableDictionary<TerrainLevel, VertexType> VertexTypeAtTerrainLevel = new();
    private Dictionary<IVertexTypeCondition, VertexType> _terrainLevelConditions = new();

    public void Init() 
    {
        foreach(var keyPair in VertexTypeAtTerrainLevel.ToDictionary())
        {
            var terrainLevel = keyPair.Key;
            var vertexType = keyPair.Value;

            _terrainLevelConditions.Add(terrainLevel.ToTerrainLevelCondition(), vertexType);
        }
    }

    public VertexType Select(int vertexPositionY, int columnHeight) 
    {
        foreach (var keyPair in _terrainLevelConditions)
        {
            var condition = keyPair.Key;
            var vertexType = keyPair.Value;

            if (condition.IsTerrainLevel(vertexPositionY, columnHeight)) return vertexType;
        }

        return VertexType.Dirt;
    }
}

public interface IVertexTypeCondition
{
    bool IsTerrainLevel(int vertexPositionY, int columnHeight);
}

class SurfaceTerrainLevelCondition : IVertexTypeCondition
{
    public bool IsTerrainLevel(int vertexPositionY, int columnHeight) => vertexPositionY <= columnHeight && vertexPositionY >= columnHeight - 3;
}

class FloorTerrainLevelCondition : IVertexTypeCondition
{
    public bool IsTerrainLevel(int vertexPositionY, int columnHeight) => vertexPositionY <= 2;
}

class MiddleGroundTerrainLevelCondition : IVertexTypeCondition
{
    public bool IsTerrainLevel(int vertexPositionY, int columnHeight) => vertexPositionY > 0 && vertexPositionY < columnHeight - 3;
}