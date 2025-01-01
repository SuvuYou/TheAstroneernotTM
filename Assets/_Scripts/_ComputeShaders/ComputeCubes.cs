using UnityEngine;

struct Triangle {
    public Vector3 VertexA;
    public Vector3 VertexB;
    public Vector3 VertexC;
};

public class ComputeCubes : MonoBehaviour
{
    // WARNING: Needs to be the same as in the MarchingCubesConst.hlsl
    private static readonly int NUM_THREADS = 8;

    [SerializeField]
    private ComputeShader _computeShader;

    private ComputeBuffer _verticesBuffer;
    private ComputeBuffer _trianglesBuffer;
    private ComputeBuffer _trianglesCountBuffer;

    private PreallocatedArray<Triangle> _triangles = new(12000);

    private void Awake()
    {
        _createBuffers();
    }

    public void ComputeTriangleVertices(Vector4[] verticesWithActivation, ref PreallocatedArray<Vector3> outputVertices)
    {
        _computeTrianglesOnGPU(verticesWithActivation);

        _extractTriangleVertices(ref outputVertices);
    }

    private void _computeTrianglesOnGPU(Vector4[] verticesWithActivation)
    {
        int numberOfThreadsGroupsHorizontally = Mathf.CeilToInt((WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS) / NUM_THREADS);
        int numberOfThreadsGroupsVertically = Mathf.CeilToInt((WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS) / NUM_THREADS);

        _verticesBuffer.SetData(verticesWithActivation);

        int kernelID = _computeShader.FindKernel("March");

        _computeShader.SetBuffer(kernelID, "vertices", _verticesBuffer);
        _computeShader.SetBuffer(kernelID, "triangles", _trianglesBuffer);
        _computeShader.SetFloat("activationThreashold", WorldDataSinglton.Instance.ACTIVATION_THRESHOLD);
        _computeShader.SetInt("numberOfVerticesHorizontally", WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS);
        _computeShader.SetInt("numberOfVerticesVertically", WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS);

        _trianglesBuffer.SetCounterValue(0);

        _computeShader.Dispatch(kernelID, numberOfThreadsGroupsHorizontally, numberOfThreadsGroupsVertically, numberOfThreadsGroupsHorizontally);

        // Get the count of generated triangles
        int[] triangleCountArray = new int[1];
        ComputeBuffer.CopyCount(_trianglesBuffer, _trianglesCountBuffer, 0);
        
        _trianglesCountBuffer.GetData(triangleCountArray);
        int triangleCount = triangleCountArray[0]; 

        // Retrieve the actual triangle data
        _trianglesBuffer.GetData(_triangles.FullArray, 0, 0, triangleCount);
        _triangles.SetCount(triangleCount);
    }

    private void _extractTriangleVertices(ref PreallocatedArray<Vector3> outputVertices)
    {
        for(int i = 0; i < _triangles.Count; i++)
        {
            outputVertices.Add(_triangles.FullArray[i].VertexA);
            outputVertices.Add(_triangles.FullArray[i].VertexB);
            outputVertices.Add(_triangles.FullArray[i].VertexC);
        }
    }

    private void _createBuffers()
    {
        int verticesCount = (int)(Mathf.Pow(WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS, 2) * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS);
        int maxNumberOfTriangles = verticesCount * 5;

        _verticesBuffer = new ComputeBuffer(verticesCount, sizeof(float) * 4);
        _trianglesBuffer = new ComputeBuffer(maxNumberOfTriangles, sizeof(float) * 3 * 3, ComputeBufferType.Append);
        _trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        _trianglesBuffer.SetCounterValue(0);
    }

    private void OnDestroy()
    {
        _verticesBuffer?.Release();
        _trianglesBuffer?.Release();
        _trianglesCountBuffer?.Release();
    }
}
