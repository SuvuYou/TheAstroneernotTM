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

    public Func<CustomVertex, bool> GetConditionFunction (Vector3 spherePosition) => (vertex) =>_sqrDistance(spherePosition, vertex) <= SphereRadius * SphereRadius;

    private float _sqrDistance(Vector3 pos, CustomVertex vertex)
    {
        var numx = pos.x - vertex.x;
        var numy = pos.y - vertex.y;
        var numz = pos.z - vertex.z;

        return numx * numx + numy * numy + numz * numz;
    }

    public void Activate() => gameObject.SetActive(true);

    public void Deactivate() => gameObject.SetActive(false);

    private void _updateSphereRadious()
    {
        SphereRadius += _playerInputValues.MouseMovementInput.x * _sphereRadiusSensitivity;

        if (SphereRadius <= 0.1f) SphereRadius = 0.1f;

        SphereRadius = 10f;

        transform.localScale = new Vector3(SphereRadius, SphereRadius, SphereRadius);
    }

    public Vector3Int GetLowerSphereBounds() => new ((int)transform.position.x - (int)SphereRadius, (int)transform.position.y - (int)SphereRadius, (int)transform.position.z - (int)SphereRadius);

    public Vector3Int GetUpperSphereBounds() => new ((int)transform.position.x + (int)SphereRadius, (int)transform.position.y + (int)SphereRadius, (int)transform.position.z + (int)SphereRadius);
}
