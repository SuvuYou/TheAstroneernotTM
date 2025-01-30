using UnityEngine;

class AttackController : MonoBehaviour
{
    [SerializeField]
    private ThrowProjectileAttack _attack;

    public void Attack() => _attack.Attack();

    public void CancelAim() => _attack.CancelAim();

    public void Aim() => _attack.Aim();
}



