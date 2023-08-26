using UnityEngine;

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

    public override void Attack(Zombie target)
    {
        if (target == null || !target.IsAvailable())
        {
            return;
        }


        switch (_rangedType)
        {
            case DefenderRangedType.Archer:
                _animationHelper.PlayAnimation(AnimationType.AttackArcher);
                break;
        }
        AudioSource.PlayClipAtPoint(_attackSound, transform.position);
        _target = target.transform;
    }

    public void ShootProjectile()
    {
        Projectile projectile = Instantiate(_projectile, ProjectileSpawnPosition.position, Quaternion.identity);
        projectile.SetTarget(_target, _projectileSpeed, Data.Damage);
        AudioSource.PlayClipAtPoint(AttackSound, transform.position);
    }
}
