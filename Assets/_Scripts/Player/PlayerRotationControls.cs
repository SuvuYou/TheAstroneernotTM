using UnityEngine;

class PlayerRotationControls : MonoBehaviour
{
    [SerializeField]
    private Transform _playerTransform;
    
    [SerializeField]
    private Transform _playerFocalPointTransform;

    [SerializeField]
    private CameraInputValues _cameraInputValues;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        var cameraRotation = Quaternion.LookRotation(_cameraInputValues.CameraLookDirection);
        
        _playerFocalPointTransform.rotation = cameraRotation;
        _playerTransform.eulerAngles = new Vector3(0, cameraRotation.eulerAngles. y, 0);
    }
}