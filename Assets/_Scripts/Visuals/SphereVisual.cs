using System;
using UnityEngine;

public class SphereVisual : MonoBehaviour
{
    [SerializeField]
    private PlayerInputValues _playerInputValues;
    
    public float SphereRadius { get; private set; } = 10f;

    private float _sphereRadiusSensitivity = 1f;

    private void Update()
    {
        if (_playerInputValues.IsHoldingRightMouseButton)
            _updateSphereRadious();
    }

    public void MoveTo(Vector3 targetPosition) => transform.position = targetPosition;
    
    public bool IsVertexInSphere(Vector3Int vertex) => Vector3.Distance(transform.position, vertex) <= SphereRadius;

    public void Activate() => gameObject.SetActive(true);

    public void Deactivate() => gameObject.SetActive(false);

    private void _updateSphereRadious()
    {
        SphereRadius += _playerInputValues.MouseMovementInput.x * _sphereRadiusSensitivity;

        if (SphereRadius <= 0.1f) SphereRadius = 0.1f;

        transform.localScale = new Vector3(SphereRadius, SphereRadius, SphereRadius);
    }
}
