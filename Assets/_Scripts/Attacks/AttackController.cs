using UnityEngine;

[RequireComponent(typeof(IAttackType))]
class AttackController : MonoBehaviour
{
    private IAttackType _attack;

    private void Awake() => _attack = GetComponent<IAttackType>();

    public void Attack() => _attack.Attack();

    public void CancelAim() => _attack.CancelAim();

    public void Aim() => _attack.Aim();
}
