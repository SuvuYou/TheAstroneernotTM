using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 _transformOffset;
    
    [SerializeField]
    private Vector3 _rotationOffset;

    [SerializeField]
    private float _interpolationSpeed = 16f;

    [SerializeField]
    private bool _rotateWithTarget = false;

    [SerializeField]
    private bool _interpolationPosition = false;

    [SerializeField]
    private bool _interpolationRotation = false;

    private void LateUpdate()
    {
        if (_rotateWithTarget)
            _followPositionWithRotation();
        else
            _followPosition();
    }

    private void _followPosition()
    {
        _setTransform(target.position + _transformOffset);
    }

    private void _followPositionWithRotation()
    {
        var rotation = target.rotation * Quaternion.Euler(_rotationOffset);
        var lookDirection = rotation * Vector3.forward;
        var position = target.position - lookDirection * _transformOffset.magnitude;

        _setTransform(position, rotation);
    }

    private void _setTransform(Vector3 position)
    {
        var pos = position;

        if (_interpolationPosition)
            pos = Vector3.Lerp(transform.position, position, _interpolationSpeed * Time.deltaTime);
        
        transform.position = pos;
    }

    private void _setTransform(Vector3 position, Quaternion rotation)
    {
        var pos = position;
        var rot = rotation;

        if (_interpolationPosition)
            pos = Vector3.Lerp(transform.position, position, _interpolationSpeed * Time.deltaTime);
        
        if (_interpolationRotation)
            rot = Quaternion.Lerp(transform.rotation, rotation, _interpolationSpeed * Time.deltaTime);
        
        transform.SetPositionAndRotation(pos, rot);
    }
}
