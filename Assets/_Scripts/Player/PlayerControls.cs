using System;
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
        {
            _updateActivationValueInSphereRadious();
        }
    }

    private void _updateActivationValueInSphereRadious()
    {
        var vertices = _worldRef.GetVerticesInRadius(_sphereVisual.transform.position, _sphereVisual.SphereRadius);
        
        float value = WorldDataSinglton.Instance.ACTIVATION_THRESHOLD * Time.deltaTime;

        if (_isAddingMode)
            value *= -1;

        _worldRef.AddVerticesActivation(vertices, value);
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
