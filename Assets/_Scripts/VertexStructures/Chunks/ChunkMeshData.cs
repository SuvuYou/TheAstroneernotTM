public class ChunkMeshData : MeshData
{
    private const int CHUNK_VERTICES_LIMIT = 12000;

    public ChunkMeshData() : base(CHUNK_VERTICES_LIMIT) { }
}
