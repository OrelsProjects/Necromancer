using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum DefenderRangedType
{
    Archer,
}

public class DefenderRanged : Defender
{
    [Header("Ranged Defender Stats")]
    [SerializeField]
    private Projectile _projectile;
    [SerializeField]
    private float _projectileSpeed;
    [SerializeField]
    private DefenderRangedType _rangedType;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip _attackSound;

    private Transform _target;

    public override AudioClip AttackSound
    {
        get { return _attackSound; }
        set { _attackSound = value; }
    }

    public override void Attack(Transform target)
    {
        switch (_rangedType)
        {
            case DefenderRangedType.Archer:
                _animationHelper.PlayAnimation(AnimationType.AttackArcher);
                break;
        }
        _target = target;
    }

    public void ShootProjectile()
    {
        Projectile projectile = Instantiate(_projectile, transform.position, Quaternion.identity);
        projectile.SetTarget(_target, _projectileSpeed);
    }
}
