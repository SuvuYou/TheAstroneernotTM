using UnityEngine;

class PlayerAttackHandler : MonoBehaviour
{
    [SerializeField]
    private AttackController _attackController;

    [SerializeField]
    private PlayerInputValues _playerInputValues;

    private void OnEnable()
    {
        _playerInputValues.OnRightMouseButtonDown += () => _attackController.Aim();
        _playerInputValues.OnRightMouseButtonUp += () => _attackController.CancelAim();
        _playerInputValues.OnLeftMouseButtonDown += () => _attackController.Attack();
    }

    private void OnDisable()
    {
        _playerInputValues.OnRightMouseButtonDown -= () => _attackController.Aim();
        _playerInputValues.OnRightMouseButtonUp -= () => _attackController.CancelAim();
        _playerInputValues.OnLeftMouseButtonDown -= () => _attackController.Attack();
    }
}



