using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
class PlayerMovement : MonoBehaviour 
{
    private Rigidbody _playerRigidbody;

    [SerializeField]
    private CameraInputValues _cameraInputValues;

    [SerializeField]
    private PlayerInputValues _playerInputValues;

    [SerializeField]
    private float _jumpForce = 25f;

    [SerializeField]
    private float _movementSpeed = 75f;

    [SerializeField]
    private float _maxMovementSpeed = 25f;

    [SerializeField]
    private float _groundCheckRaycastDistance = 0.5f;

    private bool _shouldJumpNextUpdate = false;

    private bool _isGrounded = false;

    private Timer _cayoteJumpTimer;

    private void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody>();

        _cayoteJumpTimer = Timer.CreateInstance();
        _cayoteJumpTimer.Init(duration: 0.5f, deactivateAfterTimerEnd: true, onTimerEnd: () => _isGrounded = false);
    }

    private void Update()
    {
        _performGroundCheck();
        _collectInput();
        _capVelocity();      
    }

    private void FixedUpdate()
    {
        _applyMovementForce();
        _applyJumpForce();
    }

    private void _applyMovementForce() 
    {
        var localMovementDirection = GetLocalVectorDirection(_playerInputValues.MovementInput.normalized, new Vector2(_cameraInputValues.CameraLookDirection.x, _cameraInputValues.CameraLookDirection.z));

        _playerRigidbody.AddForce(new Vector3(localMovementDirection.x, 0, localMovementDirection.y)  * _movementSpeed);
    }
    
    private void _applyJumpForce() 
    {
        if (_shouldJumpNextUpdate)
        {
            _playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _shouldJumpNextUpdate = false;
        }
    }

    private void _collectInput()
    {
        if (_playerInputValues.IsSpaceDown && _isGrounded)
        {
            _shouldJumpNextUpdate = true;
        }
    }

    private void _capVelocity()
    {
        _playerRigidbody.velocity = Vector3.ClampMagnitude(_playerRigidbody.velocity, _maxMovementSpeed);
    }

    private void _performGroundCheck()
    {
        Vector3 originOffset = Vector3.up * 0.01f;
        _setIsGroundedWithCayoteTime(isGrounded: false);

        if (Physics.Raycast(transform.position + originOffset, Vector3.down, out RaycastHit _, _groundCheckRaycastDistance))
        {
            _setIsGroundedWithCayoteTime(isGrounded: true);
        }
    }

    private void _setIsGroundedWithCayoteTime(bool isGrounded) 
    {
        if (isGrounded) 
        {
            _isGrounded = isGrounded;
            _cayoteJumpTimer.Deactivate();
        }
        else
        {
            _cayoteJumpTimer.Activate();
        }
    }

    private Vector2 GetLocalVectorDirection(Vector2 vectorDirection, Vector2 coordinateSpaceVector)
    {
        var normalCoordinateSpaceVector = coordinateSpaceVector.normalized;

        Vector2 perpendicularRightVector = new (normalCoordinateSpaceVector.y, -normalCoordinateSpaceVector.x);

        Vector2 localY = vectorDirection.y * normalCoordinateSpaceVector;
        Vector2 localX = vectorDirection.x * perpendicularRightVector;  

        return (localY + localX).normalized;
    }
}