using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInputValues", menuName = "ScriptableObjects/PlayerInputValues")]
class PlayerInputValues : ScriptableObject
{
    public Vector2 MouseMovementInput { get; private set; }
    public Vector2 MovementInput { get; private set; }

    public bool IsHoldingRightMouseButton { get; private set; }
    public bool IsHoldingLeftMouseButton { get; private set; }

    public void SetMouseMovementInput(Vector2 mouseMovementInput) => MouseMovementInput = mouseMovementInput;
    public void SetMovementInput(Vector2 movementInput) => MovementInput = movementInput;

    public void SetIsHoldingRightMouseButton(bool isHoldingRightMouseButton) => IsHoldingRightMouseButton = isHoldingRightMouseButton;
    public void SetIsHoldingLeftMouseButton(bool isHoldingLeftMouseButton) => IsHoldingLeftMouseButton = isHoldingLeftMouseButton;
}