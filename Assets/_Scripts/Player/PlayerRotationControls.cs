using UnityEngine;

class PlayerRotationControls : MonoBehaviour
{
    [SerializeField]
    private Transform _playerVisualTransform;
    
    [SerializeField]
    private Transform _playerFocalPointTransform;

    [SerializeField]
    private CameraInputValues _cameraInputValues;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        var cameraRotation = Quaternion.LookRotation(_cameraInputValues.CameraLookDirection);

        _playerVisualTransform.eulerAngles = new Vector3(0, cameraRotation.eulerAngles.y, 0);  
        _playerFocalPointTransform.rotation = cameraRotation;
    }
}