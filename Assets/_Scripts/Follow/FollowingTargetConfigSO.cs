using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/FollowingTargetConfigSO", fileName = "FollowingTargetConfigSO")]
public class FollowingTargetConfigSO : ScriptableObject
{
    public float TransformMagniture;
    public Vector3 TransformOffset;
    public Vector3 RotationOffset;

    [Header("Strategies")]
    public bool UseConstrainedPosition = false;
    public bool ShouldInterpolatePosition = false;
    public bool ShouldInterpolateRotation = false;

    [Header("Interpolation Settings")]
    public float InterpolationSpeed = 16f;
    public float InterpolationTime = 0.1f;

    [Header("Object Constrain Settings")]
    public LayerMask ObjectsConstrainMask;
    public float MaxObjectConstrainRayDistance = 100f;
    public float ObjectsConstrainMagnitudeMargin = 1f;
}