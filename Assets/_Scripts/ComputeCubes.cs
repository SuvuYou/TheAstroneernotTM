using System.Collections.Generic;
using System.Linq;
using UnityEngine;

struct Triangle {
    public Vector3 VertexA;
    public Vector3 VertexB;
    public Vector3 VertexC;
};

public class ComputeCubes : MonoBehaviour
{
    // Needs to be the same as in the MarchingCubesConst.hlsl
    private static readonly int NUM_THREADS = 8;

    [SerializeField]
    private ComputeShader _computeShader;

    private ComputeBuffer _verticesBuffer;
    private ComputeBuffer _trianglesBuffer;
    private ComputeBuffer _trianglesCountBuffer;

    private void Awake()
    {
        _createBuffers();
    }

    public List<Vector3> ComputeTriangleVertices(Dictionary<Vector3Int, float> verticesActivationValues)
    {
        Triangle[] triangles = _computeTrianglesOnGPU(verticesActivationValues);

        return _extractTriangleVertices(triangles);
    }

    private Triangle[] _computeTrianglesOnGPU(Dictionary<Vector3Int, float> verticesActivationValues)
    {
        int numberOfThreadsGroupsPerAxis = Mathf.CeilToInt((WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS) / NUM_THREADS);

        List<Vector4> verticesWithActivation = new ();

        verticesWithActivation = verticesActivationValues.Keys.Select((Vector3Int vertex) => new Vector4(vertex.x, vertex.y, vertex.z, verticesActivationValues[vertex])).ToList();
        
        _verticesBuffer.SetData(verticesWithActivation);

        int kernelID = _computeShader.FindKernel("March");

        _computeShader.SetBuffer(kernelID, "vertices", _verticesBuffer);
        _computeShader.SetBuffer(kernelID, "triangles", _trianglesBuffer);
        _computeShader.SetFloat("activationThreashold", WorldDataSinglton.Instance.ACTIVATION_THRESHOLD);
        _computeShader.SetInt("numberOfVerticesPerAxis", WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS);

        _trianglesBuffer.SetCounterValue(0);

        _computeShader.Dispatch(kernelID, numberOfThreadsGroupsPerAxis, numberOfThreadsGroupsPerAxis, numberOfThreadsGroupsPerAxis);

        // Get the count of generated triangles
        int[] triangleCountArray = new int[1];
        ComputeBuffer.CopyCount(_trianglesBuffer, _trianglesCountBuffer, 0);
        
        _trianglesCountBuffer.GetData(triangleCountArray);
        int triangleCount = triangleCountArray[0]; 

        // Retrieve the actual triangle data
        Triangle[] triangles = new Triangle[triangleCount];
        _trianglesBuffer.GetData(triangles, 0, 0, triangleCount);

        return triangles;
    }

    private List<Vector3> _extractTriangleVertices(Triangle[] triangles)
    {
        List<Vector3> vertices = new();

        for(int i = 0; i < triangles.Length; i++)
        {
            vertices.Add(triangles[i].VertexA);
            vertices.Add(triangles[i].VertexB);
            vertices.Add(triangles[i].VertexC);
        }

        return vertices;
    }

    private void _createBuffers()
    {
        int verticesCount = (int)Mathf.Pow(WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS, 3);
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
