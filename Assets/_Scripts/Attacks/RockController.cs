using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Rock))]
class RockController : MonoBehaviour
{
    private enum RockStates
    {
        Idle,
        MovingTowardsTarget,
        Held,
        Falling,
        Launched
    }

    public Rock RockComponent { get; private set; }

    private Rigidbody _rb;
    private RockStates _state;

    private Transform _target;
    private float _elapsedTime;
    private float _currentStepPercentage;

    [SerializeField]
    private float _moveToTargetDuration = 0.5f;

    private void Awake()
    {
        RockComponent = GetComponent<Rock>();
        _rb = GetComponent<Rigidbody>();
        _state = RockStates.Idle;
        RockComponent.Init();
    }

    public void MoveTowardsTarget(Transform targetPosition)
    {
        _target = targetPosition;
        _state = RockStates.MovingTowardsTarget;
    }

    public void Fall()
    {
        _changeState(RockStates.Falling);
    }

    public void Launch()
    {
        _changeState(RockStates.Launched);
        _rb.AddForce(_target.forward * 75f, ForceMode.Impulse);
    }

    private void _changeState(RockStates state)
    {
        _state = state;

        switch (_state)
        {
            case RockStates.Idle:
                break;
            case RockStates.MovingTowardsTarget:
                _rb.isKinematic = true;
                _moveToTarget();
                break;
            case RockStates.Held:
                _rb.isKinematic = true;
                break;
            case RockStates.Falling:
                _rb.isKinematic = false;
                break;
            case RockStates.Launched:
                _rb.isKinematic = false;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        switch (_state)
        {
            case RockStates.MovingTowardsTarget:
                _moveToTarget();
                break;
            case RockStates.Held:
                _wobble();
                _checkDistanceToTarget();
                break;
            default:
                break;
        }
    }
    
    private void _moveToTarget()
    {
        _elapsedTime += Time.deltaTime;

        _currentStepPercentage = Mathf.SmoothStep(0, 1, _elapsedTime / _moveToTargetDuration);

        transform.position = Vector3.Lerp(transform.position, _target.position, _currentStepPercentage);

        _wobble();

        if (_elapsedTime > _moveToTargetDuration)
        {
            _changeState(RockStates.Held);

            _elapsedTime = 0;
        }
    }

    private void _checkDistanceToTarget()
    {
        if(Vector3.Distance(transform.position, _target.position) > 0.5f)
        {
            _changeState(RockStates.MovingTowardsTarget);
        }
    }

    private void _wobble()
    {
        transform.position += new Vector3(
            Mathf.Sin(Time.time * 8f) * 0.08f,
            Mathf.Cos(Time.time / 11f) * 0.08f,
            0
        ) * Time.deltaTime;
    }
}