using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
class CustomGravity : MonoBehaviour 
{
    private const float GLOBAL_GRAVITY_FORCE_MAGNITUDE = 9.8f;

    private Rigidbody _rigidbody;

    [SerializeField]
    private float _defaultGravityScale = 2f;
    
    [SerializeField]
    private float _fallingGravityScale = 5f;

    private float _gravityScale;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
    }

    private void Update()
    {
        _updateGravityScale();  
    }

    private void FixedUpdate()
    {
        _applyGravityForce();
    }

    private void _applyGravityForce() => _rigidbody.AddForce(Vector3.down * (GLOBAL_GRAVITY_FORCE_MAGNITUDE * _gravityScale), ForceMode.Acceleration);

    private void _updateGravityScale() 
    {
        if (_rigidbody.velocity.y < 0) _gravityScale = _fallingGravityScale;
        else _gravityScale = _defaultGravityScale;
    }
}