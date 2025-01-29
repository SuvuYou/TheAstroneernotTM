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
    Quaternion GetRotation(Transform target, Quaternion offset, float interpolationSpeed);
}

public class SimpleRotationFollowStrategy : IRotationFollowStrategy
{
    public Quaternion GetRotation(Transform target, Quaternion offset, float interpolationSpeed)
    {
        return target.rotation * offset;
    }
}

public class InterpolatedRotationFollowStrategy : IRotationFollowStrategy
{
    public Quaternion GetRotation(Transform target, Quaternion offset, float interpolationSpeed)
    {
        return Quaternion.Lerp(target.rotation, target.rotation * offset, interpolationSpeed * Time.deltaTime);
    }
}

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private FollowingTargetConfigSO _followingTargetConfig;

    [SerializeField]
    private float _offsetInterpolationTime = 0.2f;

    private IPositionFollowStrategy _positionStrategy;
    private IRotationFollowStrategy _rotationStrategy;

    private Vector3 _interpolationVelocity;
    private Vector3 _ofsetInterpolationVelocity;

    private Vector3 _cachedOffset;

    private void Awake()
    {
        _positionStrategy = _followingTargetConfig.UseConstrainedPosition
            ? new ConstrainedPositionFollowStrategy()
            : new SimplePositionFollowStrategy();

        _rotationStrategy = _followingTargetConfig.ShouldInterpolateRotation
            ? new InterpolatedRotationFollowStrategy()
            : new SimpleRotationFollowStrategy();

        _cachedOffset = _followingTargetConfig.TransformOffset;
    }

    public void SwitchConfig(FollowingTargetConfigSO followingTargetConfig) 
    {
        _followingTargetConfig = followingTargetConfig;
    }

    private void LateUpdate()
    {
        _followTargetWithStrategy();
    }

    private void _followTargetWithStrategy()
    {
        _cachedOffset = _interpolateOffsetTransform(_cachedOffset, _followingTargetConfig.TransformOffset);

        var rotation = _rotationStrategy.GetRotation(_target, Quaternion.Euler(_followingTargetConfig.RotationOffset), _followingTargetConfig.InterpolationSpeed);

        var rotationTransform = (rotation * Vector3.back).normalized * _followingTargetConfig.TransformMagniture;
        var offsetTransform = (rotation * _cachedOffset).normalized * _cachedOffset.magnitude;

        var position = _positionStrategy.GetPosition(_target, rotationTransform + offsetTransform, _followingTargetConfig.ObjectsConstrainMask, _followingTargetConfig.MaxObjectConstrainRayDistance, _followingTargetConfig.ObjectsConstrainMagnitudeMargin);

        position = _followingTargetConfig.ShouldInterpolatePosition ? _interpolatePosition(transform.position, position) : position;

        transform.SetPositionAndRotation(position, rotation);
    }

    private Vector3 _interpolatePosition(Vector3 currentPosition, Vector3 targetPosition)
    {
        return Vector3.SmoothDamp(currentPosition, targetPosition, ref _interpolationVelocity, _followingTargetConfig.InterpolationTime);
    }

    private Vector3 _interpolateOffsetTransform(Vector3 currentOffset, Vector3 targetOffset)
    {
        return Vector3.SmoothDamp(currentOffset, targetOffset, ref _ofsetInterpolationVelocity, _offsetInterpolationTime);
    }
}
