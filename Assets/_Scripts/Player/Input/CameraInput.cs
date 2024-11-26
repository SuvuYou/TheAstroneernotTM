using System;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    [SerializeField]
    private Transform _cameraTransform;

    [SerializeField]
    private float _cameraSensitivity = 1f;

    [SerializeField]
    private PlayerInputValues _playerInputValues;

    [SerializeField]
    private CameraInputValues _cameraInputValues;

    [SerializeField]
    [Range(0, 1)]
    private float _cameraYRotationLimit = 0.8f;

    private void Start()
    {
        _cameraInputValues.SetCameraLookDirection(_cameraTransform.forward);
    }

    private void Update()
    {
        var rotationQuaternian = _calculateRotationQuaternian(_playerInputValues.MouseMovementInput, _cameraTransform);
        var lookDirection = _aplyRotationWithLimitToRotationAngle(rotationQuaternian, _cameraInputValues.CameraLookDirection);

        _cameraInputValues.SetCameraLookDirection(lookDirection);
    }

    private Quaternion _calculateRotationQuaternian(Vector2 rotationDirection, Transform objectReferenceTransform) 
    {
        var xRotationMagnitude = rotationDirection.x * _cameraSensitivity * Time.deltaTime;
        var yRotationMagnitude = rotationDirection.y * _cameraSensitivity * Time.deltaTime;

        yRotationMagnitude = _limitYRotation(yRotationMagnitude);

        Quaternion horizontalRotation = Quaternion.AngleAxis(xRotationMagnitude, objectReferenceTransform.up);
        Quaternion verticalRotation = Quaternion.AngleAxis(-yRotationMagnitude, objectReferenceTransform.right);

        return horizontalRotation * verticalRotation;
    }

    private Vector3 _aplyRotationWithLimitToRotationAngle(Quaternion rotationQuaternian, Vector3 lookDirection)
    {
        var directionVector = rotationQuaternian * lookDirection;
        
        directionVector.y = Mathf.Clamp(directionVector.y, -_cameraYRotationLimit, _cameraYRotationLimit);

        return directionVector.normalized;
    }

    private float _limitYRotation(float yRotationMagnitude) 
    {
        if (_cameraInputValues.CameraLookDirection.y >= _cameraYRotationLimit && yRotationMagnitude > 0) 
        {
            yRotationMagnitude = 0;
        }
        
        if (_cameraInputValues.CameraLookDirection.y <= -_cameraYRotationLimit && yRotationMagnitude < 0) 
        {
            yRotationMagnitude = 0;
        }

        return yRotationMagnitude;
    }
}
