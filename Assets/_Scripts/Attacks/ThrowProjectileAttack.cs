using UnityEngine;

interface IAttackType
{
    public abstract void Aim();

    public abstract void CancelAim();

    public abstract void Attack();
}

class ThrowProjectileAttack : MonoBehaviour, IAttackType
{
    [SerializeField]
    private RockController _projectilePrefab;

    private RockController _projectile;

    [SerializeField]
    private Transform _rockSpawnPosition;

    [SerializeField]
    private Transform _rockHoldPosition;

    public void Aim() 
    { 
        _projectile = Instantiate(_projectilePrefab, _rockSpawnPosition.position, Quaternion.identity);
        _projectile.MoveTowardsTarget(_rockHoldPosition);
    }

    public void CancelAim()
    {
        _projectile?.Fall();
    }

    public void Attack()
    {
        _projectile?.Launch();
    }
}