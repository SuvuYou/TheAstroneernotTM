using UnityEngine;

struct Triangle {
    public Vector3 VertexA;
    public Vector3 VertexB;
    public Vector3 VertexC;
};

struct TriangleData {
    public Triangle triangle;
    public int vertexIndex;
}

public class ComputeCubes : MonoBehaviour
{
    // WARNING: Needs to be the same as in the MarchingCubesConst.hlsl
    private static readonly int NUM_THREADS = 8;

    [SerializeField]
    private ComputeShader _computeShader;

    [SerializeField]
    private VertexTypeMaterialsManagerSO _materialManager;

    private ComputeBuffer _verticesBuffer;
    private ComputeBuffer _triangleDataBuffer;
    private ComputeBuffer _trianglesCountBuffer;

    private PreallocatedArray<TriangleData> _triangleData = new(5000);

    private void Awake()
    {
        _createBuffers();
    }

    public void ComputeTriangleVertices(Vertex[] vertices, Vector4[] verticesWithActivation, ref PreallocatedArray<Vector3> outputVertices, ref PreallocatedArray<Vector2> outputUVs)
    {
        _computeTrianglesOnGPU(verticesWithActivation);

        _extractTriangleVertices(vertices, ref outputVertices, ref outputUVs);
    }

    private void _computeTrianglesOnGPU(Vector4[] verticesWithActivation)
    {
        int numberOfThreadsGroupsHorizontally = Mathf.CeilToInt((WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS) / NUM_THREADS);
        int numberOfThreadsGroupsVertically = Mathf.CeilToInt((WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS) / NUM_THREADS);

        _verticesBuffer.SetData(verticesWithActivation);

        int kernelID = _computeShader.FindKernel("March");

        _computeShader.SetBuffer(kernelID, "vertices", _verticesBuffer);
        _computeShader.SetBuffer(kernelID, "triangleData", _triangleDataBuffer);
        _computeShader.SetFloat("activationThreashold", WorldDataSinglton.Instance.ACTIVATION_THRESHOLD);
        _computeShader.SetInt("numberOfVerticesHorizontally", WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS);
        _computeShader.SetInt("numberOfVerticesVertically", WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS);

        _triangleDataBuffer.SetCounterValue(0);

        _computeShader.Dispatch(kernelID, numberOfThreadsGroupsHorizontally, numberOfThreadsGroupsVertically, numberOfThreadsGroupsHorizontally);

        // Get the count of generated triangles
        int[] triangleCountArray = new int[1];
        ComputeBuffer.CopyCount(_triangleDataBuffer, _trianglesCountBuffer, 0);
        _trianglesCountBuffer.GetData(triangleCountArray);
        int triangleCount = triangleCountArray[0];

        _triangleData.SetCount(triangleCount);

        // Retrieve the actual triangle data
        _triangleDataBuffer.GetData(_triangleData.FullArray, 0, 0, triangleCount);
    }

    private void _extractTriangleVertices(Vertex[] vertices, ref PreallocatedArray<Vector3> outputVertices, ref PreallocatedArray<Vector2> outputUVs)
    {
        for (int i = 0; i < _triangleData.Count; i++)
        {
            TriangleData data = _triangleData.FullArray[i];
            outputVertices.AddWithResize(data.triangle.VertexA);
            outputVertices.AddWithResize(data.triangle.VertexB);
            outputVertices.AddWithResize(data.triangle.VertexC);

            Rect uvs = _materialManager.GetMaterialUVsByVertexType(vertices[data.vertexIndex].Type);

            Vector2 uvCenter = new(uvs.center.x, uvs.center.y);

            outputUVs.AddWithResize(uvCenter);
            outputUVs.AddWithResize(uvCenter);
            outputUVs.AddWithResize(uvCenter);
        }
    }

    private void _createBuffers()
    {
        int verticesCount = (int)(Mathf.Pow(WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS, 2) * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS);
        int maxNumberOfTriangles = verticesCount * 5;

        _verticesBuffer = new ComputeBuffer(verticesCount, sizeof(float) * 4);
        _triangleDataBuffer = new ComputeBuffer(maxNumberOfTriangles, sizeof(float) * 3 * 3 + sizeof(int), ComputeBufferType.Append);
        _trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        _triangleDataBuffer.SetCounterValue(0);
    }

    private void OnDestroy()
    {
        _verticesBuffer?.Release();
        _triangleDataBuffer?.Release();
        _trianglesCountBuffer?.Release();
    }
}
