using UnityEngine;

public class FollowTargetSwitchOnRightClick : MonoBehaviour
{
    [SerializeField]
    private PlayerInputValues _playerInputValues;

    [SerializeField]
    private FollowTarget _followingTarget;

    [SerializeField]
    private FollowingTargetConfigSO _defaultFollowingTargetConfig;

    [SerializeField]
    private FollowingTargetConfigSO _rightClickFollowingTargetConfig;

    private void OnEnable()
    {
        _playerInputValues.OnRightMouseButtonDown += () => _followingTarget.SwitchConfig(_rightClickFollowingTargetConfig);
        _playerInputValues.OnRightMouseButtonUp += () => _followingTarget.SwitchConfig(_defaultFollowingTargetConfig);
    }

    private void OnDisable()
    {
        _playerInputValues.OnRightMouseButtonDown -= () => _followingTarget.SwitchConfig(_rightClickFollowingTargetConfig);
        _playerInputValues.OnRightMouseButtonUp -= () => _followingTarget.SwitchConfig(_defaultFollowingTargetConfig);
    } 
}
