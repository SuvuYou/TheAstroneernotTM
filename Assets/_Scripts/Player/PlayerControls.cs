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

    private void Start()
    {
        _sphereVisual = Instantiate(_sphereVisualPrefab, Vector3.zero, Quaternion.identity);
        _sphereVisual.Deactivate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            _isAddingMode = !_isAddingMode;

        if (!_playerInputValues.IsHoldingRightMouseButton)
            _moveSphereVisual();

        if (_playerInputValues.IsHoldingLeftMouseButton)
            _updateActivationValueInSphereRadious();
    }

    private void _updateActivationValueInSphereRadious()
    {
        Dictionary<Vector3Int, float> verticesActivation = new();

        foreach (var vertex in _worldRef.GetVerticesByCondition(_sphereVisual.IsVertexInSphere)) 
        {
            if (_isAddingMode)
                verticesActivation[vertex] = -WorldDataSinglton.Instance.ACTIVATION_THRESHOLD * Time.deltaTime;
            else
                verticesActivation[vertex] = WorldDataSinglton.Instance.ACTIVATION_THRESHOLD * Time.deltaTime;
        }

        _worldRef.AddVerticesActivation(verticesActivation);
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
