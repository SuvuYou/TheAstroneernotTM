using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField]
    private World _worldRef;

    [SerializeField]
    private SphereVisual _sphereVisualPrefab;

    [SerializeField]
    private PlayerInputValues _playerInputValues;

    private SphereVisual _sphereVisual;

    private bool _isAddingMode = false;

    private const int REPETITION_FRAME_COUNT = 4;
    private int _frameCounter = 0;
    private float _cumulativeFrameDeltaTime = 0;

    private void Start()
    {
        _sphereVisual = Instantiate(_sphereVisualPrefab, Vector3.zero, Quaternion.identity);
        _sphereVisual.Deactivate();
    }

    private void Update()
    {
        _cumulativeFrameDeltaTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.V))
            _isAddingMode = !_isAddingMode;

        if (!_playerInputValues.IsHoldingRightMouseButton)
            _moveSphereVisual();

        if (_playerInputValues.IsHoldingLeftMouseButton)
        {
            _frameCounter++;

            if (_frameCounter >= REPETITION_FRAME_COUNT - 1)
            {
                _updateActivationValueInSphereRadious();

                _frameCounter = 0;
                _cumulativeFrameDeltaTime = 0;
            }
        }
        else
        {
            _frameCounter = 0;
            _cumulativeFrameDeltaTime = 0;
        }
    }

    private void _updateActivationValueInSphereRadious()
    {
        Dictionary<Vector3Int, List<Vector3Int>> vertexNeighbours = new();

        foreach (var vertex in _worldRef.GetVerticesInRadius(_sphereVisual.transform.position, _sphereVisual.SphereRadius)) 
        {
            vertexNeighbours[vertex] = _getNeighbours(vertex);
        }

        float value = WorldDataSinglton.Instance.ACTIVATION_THRESHOLD * _cumulativeFrameDeltaTime * Mathf.Sqrt(REPETITION_FRAME_COUNT);

        if (_isAddingMode)
            value *= -1;

        _worldRef.AddVerticesActivation(vertexNeighbours, value);
    }

    private List<Vector3Int> _getNeighbours(Vector3Int vertex)
    {
        return new List<Vector3Int>() {
            vertex + new Vector3Int(-1, 0, 0), vertex + new Vector3Int(1, 0, 0), 
            vertex + new Vector3Int(0, 0, -1), vertex + new Vector3Int(0, 0, 1), 
            vertex + new Vector3Int(0, -1, 0), vertex + new Vector3Int(0, 1, 0),
            vertex + new Vector3Int(-1, -1, 0), vertex + new Vector3Int(1, 1, 0),
            vertex + new Vector3Int(0, -1, -1), vertex + new Vector3Int(0, 1, 1),
            vertex + new Vector3Int(-1, 0, -1), vertex + new Vector3Int(1, 0, 1),
            vertex + new Vector3Int(1, -1, 0), vertex + new Vector3Int(-1, 1, 0),
            vertex + new Vector3Int(0, 1, -1), vertex + new Vector3Int(0, -1, 1),
            vertex + new Vector3Int(1, 0, -1), vertex + new Vector3Int(-1, 0, 1),
            vertex + new Vector3Int(1, 1, 1), vertex + new Vector3Int(-1, -1, -1),
            vertex + new Vector3Int(-1, 1, 1), vertex + new Vector3Int(1, -1, -1),
            vertex + new Vector3Int(1, -1, 1), vertex + new Vector3Int(-1, 1, -1),
            vertex + new Vector3Int(1, 1, -1), vertex + new Vector3Int(-1, -1, 1),
        };
    }

    private void _moveSphereVisual()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length == 0)
        {
            _sphereVisual.Deactivate();
            
            return;
        }

        foreach(var hit in hits)
        {
            if (hit.collider.GetComponent<ChunkMeshRenderer>() != null) 
            {
                _sphereVisual.MoveTo(hit.point);
                _sphereVisual.Activate();
            }
        }
    }
}
