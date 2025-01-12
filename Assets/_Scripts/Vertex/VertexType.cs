public enum VertexType
{
    Bedrock,
    Grass,
    Dirt
}

public enum TerrainLevel 
{ 
    Surface,
    MiddleGround,
    Floor
}

public static class TerrainLevelExtensions
{
    public static IVertexTypeCondition ToTerrainLevelCondition(this TerrainLevel terrainLevel) => terrainLevel switch
    {
        TerrainLevel.Floor => new FloorTerrainLevelCondition(),
        TerrainLevel.MiddleGround => new MiddleGroundTerrainLevelCondition(),
        TerrainLevel.Surface => new SurfaceTerrainLevelCondition(),
        _ => throw new System.Exception("Unsupported Terrain Level")
    };
}

