using UnityEngine;

[CreateAssetMenu(fileName = "CameraInputValues", menuName = "ScriptableObjects/CameraInputValues")]
class CameraInputValues : ScriptableObject
{
    [SerializeField]
    private float x, y, z;

    public Vector3 CameraLookDirection { 
        get => new (x, y, z); 
        private set 
        { 
            x = value.x; 
            y = value.y; 
            z = value.z;
        } 
    }

    public void SetCameraLookDirection(Vector3 cameraLookDirection) => CameraLookDirection = cameraLookDirection;
}
