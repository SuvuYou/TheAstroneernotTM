using System.Collections.Generic;
using UnityEngine;

public class SelectVertices : MonoBehaviour
{
    // Needs to be the same as in the MarchingCubesConst.hlsl
    private static readonly int NUM_THREADS = 128;

    private ComputeBuffer _verticesBuffer;
    private ComputeBuffer _selectedVerticesBuffer;
    private ComputeBuffer _selectedVerticesCountBuffer;

    [SerializeField]
    private ComputeShader _computeShader;

    private void Start()
    {
        _createBuffers();
    }

    public Vector3Int[] SelectVerticesByCondition(List<Vector3Int> allVertices, Vector3 circleCenter, float circleRadius)
    {
        int numThreads = Mathf.CeilToInt(allVertices.Count / (float)NUM_THREADS);

        _verticesBuffer.SetData(allVertices);

        int kernelID = _computeShader.FindKernel("Select");

        _computeShader.SetBuffer(kernelID, "vertices", _verticesBuffer);
        _computeShader.SetBuffer(kernelID, "selectedVertices", _selectedVerticesBuffer);
        _computeShader.SetFloats("circleCenterPoint", circleCenter.x, circleCenter.y, circleCenter.z);
        _computeShader.SetFloat("circleRadius", circleRadius);

        _selectedVerticesBuffer.SetCounterValue(0);

        _computeShader.Dispatch(kernelID, numThreads, 1, 1);

        // Get the count of selected vertices
        ComputeBuffer.CopyCount(_selectedVerticesBuffer, _selectedVerticesCountBuffer, 0);

        int[] selectedVerticesCountArray = new int[1];
        _selectedVerticesCountBuffer.GetData(selectedVerticesCountArray);

        int selectedVerticesCount = selectedVerticesCountArray[0]; 

        // Retrieve the actual selected vertices
        Vector3Int[] selectedVertices = new Vector3Int[selectedVerticesCount];
        _selectedVerticesBuffer.GetData(selectedVertices, 0, 0, selectedVerticesCount);

        return selectedVertices;
    } 

    private void _createBuffers()
    {
        int verticesCount = (int)(Mathf.Pow(WorldDataSinglton.Instance.CHUNK_SIZE_WITH_INTERSECTIONS, 2) * WorldDataSinglton.Instance.CHUNK_HEIGHT_WITH_INTERSECTIONS) * WorldDataSinglton.Instance.RENDER_DISTANCE * WorldDataSinglton.Instance.RENDER_DISTANCE;

        _verticesBuffer = new ComputeBuffer(verticesCount, sizeof(int) * 3);
        _selectedVerticesBuffer = new ComputeBuffer(verticesCount, sizeof(int) * 3, ComputeBufferType.Append);
        _selectedVerticesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        _selectedVerticesBuffer.SetCounterValue(0);
    }

    private void OnDestroy()
    {
        _verticesBuffer?.Release();
        _selectedVerticesBuffer?.Release();
        _selectedVerticesCountBuffer?.Release();
    }
}
