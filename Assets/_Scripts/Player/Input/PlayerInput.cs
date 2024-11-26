using UnityEngine;

class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private PlayerInputValues _playerInputValues;

    private void Update()
    {
        _collectMouseMovementInput();
        _collectMouseButtonInput();
    }

    private void _collectMouseMovementInput() => _playerInputValues.SetMouseMovementInput(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

    private void _collectMouseButtonInput()
    {
        _playerInputValues.SetIsHoldingLeftMouseButton(Input.GetMouseButton(0));
        _playerInputValues.SetIsHoldingRightMouseButton(Input.GetMouseButton(1));
    }
}
