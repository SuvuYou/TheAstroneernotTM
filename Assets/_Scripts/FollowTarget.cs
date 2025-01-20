using UnityEngine;

public interface IPositionFollowStrategy
{
    Vector3 GetPosition(Transform target, Vector3 offset, LayerMask layerMask, float maxDistance, float margin);
}

public class SimplePositionFollowStrategy : IPositionFollowStrategy
{
    public Vector3 GetPosition(Transform target, Vector3 offset, LayerMask layerMask, float maxDistance, float margin)
    {
        return target.position + offset;
    }
}

public class ConstrainedPositionFollowStrategy : IPositionFollowStrategy
{
    public Vector3 GetPosition(Transform target, Vector3 offset, LayerMask layerMask, float maxDistance, float margin)
    {
        Vector3 origin = target.position;
        Vector3 direction = offset.normalized;
        Vector3 constrainedPosition = origin + offset;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask))
        {
            float constrainedDistance = (hit.point - origin).magnitude - margin;
            if (constrainedDistance < offset.magnitude)
            {
                constrainedPosition = origin + direction * constrainedDistance;
            }
        }

        return constrainedPosition;
    }
}

public interface IRotationFollowStrategy
{
    Quaternion GetRotation(Transform target, Quaternion offset, float interpolationSpeed, bool interpolate);
}

public class SimpleRotationFollowStrategy : IRotationFollowStrategy
{
    public Quaternion GetRotation(Transform target, Quaternion offset, float interpolationSpeed, bool interpolate)
    {
        return target.rotation * offset;
    }
}

public class InterpolatedRotationFollowStrategy : IRotationFollowStrategy
{
    public Quaternion GetRotation(Transform target, Quaternion offset, float interpolationSpeed, bool interpolate)
    {
        if (interpolate)
        {
            return Quaternion.Lerp(target.rotation, target.rotation * offset, interpolationSpeed * Time.deltaTime);
        }

        return target.rotation * offset;
    }
}

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 _transformOffset;
    [SerializeField] private Vector3 _rotationOffset;

    [Header("Strategies")]
    [SerializeField] private bool _useConstrainedPosition = false;
    [SerializeField] private bool _interpolateRotation = false;

    [Header("Interpolation Settings")]
    [SerializeField] private float _interpolationSpeed = 16f;

    [Header("Object Constrain Settings")]
    [SerializeField] private LayerMask _objectsConstrainMask;
    [SerializeField] private float _maxObjectConstrainRayDistance = 100f;
    [SerializeField] private float _objectsConstrainMagnitudeMargin = 1f;

    private IPositionFollowStrategy _positionStrategy;
    private IRotationFollowStrategy _rotationStrategy;

    private void Awake()
    {
        _positionStrategy = _useConstrainedPosition
            ? new ConstrainedPositionFollowStrategy()
            : new SimplePositionFollowStrategy();

        _rotationStrategy = _interpolateRotation
            ? new InterpolatedRotationFollowStrategy()
            : new SimpleRotationFollowStrategy();
    }

    private void LateUpdate()
    {
        FollowTargetWithStrategy();
    }

    private void FollowTargetWithStrategy()
    {
        var rotation = _rotationStrategy.GetRotation(target, Quaternion.Euler(_rotationOffset), _interpolationSpeed, _interpolateRotation);

        var offsetDirection = (rotation * Vector3.back).normalized;
        var position = _positionStrategy.GetPosition(target, offsetDirection * _transformOffset.magnitude, _objectsConstrainMask, _maxObjectConstrainRayDistance, _objectsConstrainMagnitudeMargin);

        transform.SetPositionAndRotation(position, rotation);
    }
}
