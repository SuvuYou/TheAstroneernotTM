using UnityEngine;

class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private PlayerInputValues _playerInputValues;

    private void Update()
    {
        _collectMouseMovementInput();
        _collectMovementInput();

        _collectIsSpaceDownInput();

        _collectMouseButtonInput();
    }

    private void _collectMouseMovementInput() => _playerInputValues.SetMouseMovementInput(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
    private void _collectMovementInput() => _playerInputValues.SetMovementInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

    private void _collectIsSpaceDownInput() => _playerInputValues.SetIsSpaceDown(Input.GetKeyDown(KeyCode.Space));

    private void _collectMouseButtonInput()
    {
        _playerInputValues.SetIsHoldingLeftMouseButton(Input.GetMouseButton(0));
        _playerInputValues.SetIsHoldingRightMouseButton(Input.GetMouseButton(1));
    }
}
