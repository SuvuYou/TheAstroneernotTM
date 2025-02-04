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
    private ComputeCubesRock _computeCubesRock;

    [SerializeField]
    private RockController _projectilePrefab;

    private RockController _projectile;

    private RockVerticesPopulator _rockVerticesPopulator;

    [SerializeField]
    private Transform _projectileSpawnPosition;

    [SerializeField]
    private Transform _projectileHoldPosition;

    public void Aim() 
    { 
        _rockVerticesPopulator = new();

        _projectile = Instantiate(_projectilePrefab, _projectileSpawnPosition.position, Quaternion.identity);

        _rockVerticesPopulator.CreateRockVertices();
        _rockVerticesPopulator.LinkVerticesToRock(_projectile.RockComponent);
        _projectile.RockComponent.GenerateMesh(_computeCubesRock);
        _projectile.MoveTowardsTarget(_projectileHoldPosition);
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