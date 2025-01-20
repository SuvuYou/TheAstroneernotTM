using System.Collections.Generic;
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
    private float _groundCheckRaycastDistance = 1f;

    [SerializeField] 
    private LayerMask _groundLayerMask;

    private Vector3 _cachedPlatformSlopeNormal;
    private Vector3 _platformSlopeNormal;
    private bool _shouldJumpNextUpdate = false;
    private bool _isGrounded = false;

    private Vector3 _movementDirection;

    private Timer _cayoteJumpTimer;

    private void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody>();

        _cayoteJumpTimer = Timer.CreateInstance();
        _cayoteJumpTimer.Init(duration: 0.5f, deactivateAfterTimerEnd: true, onTimerEnd: () => _isGrounded = false);
    }

    private void Update()
    {
        var localMovementDirection = _getLocalVectorDirection(_playerInputValues.MovementInput.normalized, new Vector2(_cameraInputValues.CameraLookDirection.x, _cameraInputValues.CameraLookDirection.z));

        _movementDirection = Vector3.ProjectOnPlane(new Vector3(localMovementDirection.x, 0, localMovementDirection.y), _platformSlopeNormal).normalized;

        _performGroundCheck();
        _collectInput();
        _capVelocity();      
        _setPlatformNormal();
    }

    private void FixedUpdate()
    {
        _applyMovementForce();
        _applyJumpForce();
    }

    private void _applyMovementForce() 
    {
        if (_platformSlopeNormal.y != 1 && Vector3.Dot(_platformSlopeNormal, Vector3.up) > 0)
        {
            var velocityMagniture = _playerRigidbody.velocity.magnitude;

            _playerRigidbody.velocity = Vector3.Lerp(_playerRigidbody.velocity.normalized, _movementDirection.normalized, 1) * velocityMagniture;
        }

        var movementSpeed = _isGrounded ? _movementSpeed : _movementSpeed * 0.25f;

        _playerRigidbody.AddForce(_movementDirection * movementSpeed);
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

        if (Physics.Raycast(transform.position + originOffset, Vector3.down, out RaycastHit _, _groundCheckRaycastDistance, _groundLayerMask))
        {
            _setIsGroundedWithCayoteTime(isGrounded: true);
        }
    }

    private void _setPlatformNormal()
    {
         Vector3 forwardDirectionOffset = _getLocalVectorDirection(_playerInputValues.MovementInput.normalized, new Vector2(_cameraInputValues.CameraLookDirection.x, _cameraInputValues.CameraLookDirection.z));
        // Vector3 forwardDirectionOffset = new Vector3(_cameraInputValues.CameraLookDirection.x, 0, _cameraInputValues.CameraLookDirection.z).normalized * 0.5f;
        // var backwardDirectionOffset = -forwardDirectionOffset;
        // var rightDirectionOffset = new Vector3(forwardDirectionOffset.z, 0, forwardDirectionOffset.x);
        // var leftDirectionOffset = -rightDirectionOffset;

        bool isHitAhead = Physics.Raycast(transform.position + forwardDirectionOffset.normalized * 0.5f + Vector3.up * 0.5f, Vector3.down, out RaycastHit platformHitAhead, _groundCheckRaycastDistance, _groundLayerMask);
        // bool isHitBehind = Physics.Raycast(transform.position + backwardDirectionOffset + Vector3.up * 0.5f, Vector3.down, out RaycastHit platformHitBehind, _groundCheckRaycastDistance, _groundLayerMask);
        // bool isHitRight = Physics.Raycast(transform.position + rightDirectionOffset + Vector3.up * 0.5f, Vector3.down, out RaycastHit platformHitRight, _groundCheckRaycastDistance, _groundLayerMask);
        // bool isHitLeft = Physics.Raycast(transform.position + leftDirectionOffset + Vector3.up * 0.5f, Vector3.down, out RaycastHit platformHitLeft, _groundCheckRaycastDistance, _groundLayerMask);
        bool isHitBeneath = Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, out RaycastHit platformHitBeneath, _groundCheckRaycastDistance, _groundLayerMask);

        List<RaycastHit> hits = new ();

        if (isHitAhead) hits.Add(platformHitAhead);
        // if (isHitBehind) hits.Add(platformHitBehind);
        // if (isHitRight) hits.Add(platformHitRight);
        // if (isHitLeft) hits.Add(platformHitLeft);
        if (isHitBeneath) hits.Add(platformHitBeneath);

        _platformSlopeNormal = Vector3.up; 

        foreach (var hit in hits)
        {
            if (_platformSlopeNormal.y > hit.normal.y) 
            {
                _cachedPlatformSlopeNormal = _platformSlopeNormal;
                _platformSlopeNormal = hit.normal; 
            }
        }
    }

    private void _redestributeVelocity(Vector3 forceDirection) 
    {
        var velocityMagniture = _playerRigidbody.velocity.magnitude;

        _playerRigidbody.velocity = forceDirection * velocityMagniture;
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

    private Vector2 _getLocalVectorDirection(Vector2 vectorDirection, Vector2 coordinateSpaceVector)
    {
        var normalCoordinateSpaceVector = coordinateSpaceVector.normalized;

        Vector2 perpendicularRightVector = new (normalCoordinateSpaceVector.y, -normalCoordinateSpaceVector.x);

        Vector2 localY = vectorDirection.y * normalCoordinateSpaceVector;
        Vector2 localX = vectorDirection.x * perpendicularRightVector;  

        return (localY + localX).normalized;
    }
}