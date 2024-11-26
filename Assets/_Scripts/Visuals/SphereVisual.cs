using UnityEngine;

public class SphereVisual : MonoBehaviour
{
    [SerializeField]
    private PlayerInputValues _playerInputValues;
    
    private float _sphereRadius = 1f;

    private float _sphereRadiusSensitivity = 1f;

    private void Update()
    {
        if (_playerInputValues.IsHoldingRightMouseButton)
            _updateSphereRadious();
    }

    public void MoveTo(Vector3 targetPosition) => transform.position = targetPosition;
    
    public bool IsVertexInSphere(Vector3Int vertex) => Vector3.Distance(transform.position, vertex) <= _sphereRadius;

    public void Activate() => gameObject.SetActive(true);

    public void Deactivate() => gameObject.SetActive(false);

    private void _updateSphereRadious()
    {
        _sphereRadius += _playerInputValues.MouseMovementInput.x * _sphereRadiusSensitivity;

        if (_sphereRadius <= 0.1f) _sphereRadius = 0.1f;

        transform.localScale = new Vector3(_sphereRadius, _sphereRadius, _sphereRadius);
    }
}
