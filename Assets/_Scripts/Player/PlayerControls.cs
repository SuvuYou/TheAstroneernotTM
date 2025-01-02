using System;
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
        {
            _updateActivationValueInSphereRadious();
        }
    }

    private void _updateActivationValueInSphereRadious()
    {
        var condition = _sphereVisual.GetConditionFunction(_sphereVisual.transform.position);

        float activationValueIncrement = WorldDataSinglton.Instance.ACTIVATION_THRESHOLD * Time.deltaTime;

        if (_isAddingMode)
            activationValueIncrement *= -1;

        _worldRef.AddActivationToVerticesByCondition(condition, lowerBounds: _sphereVisual.GetLowerSphereBounds(), upperBounds: _sphereVisual.GetUpperSphereBounds(), activationValueIncrement);
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

                break;
            }
        }
    }
}
