using UnityEngine;

public class DefenderMelee : Defender {
    [Header("Melee Defender Stats")]
    [Header("Sounds")]
    [SerializeField]
    private AudioClip _attackSound;

    private Zombie _target;

    public override AudioClip AttackSound {
        get { return _attackSound; }
        set { _attackSound = value; }
    }

    public override void Attack(Zombie target) {
        _target = target;
        _animationHelper.PlayAnimation(AnimationType.AttackMelee);
    }

    public void HitTarget() {
        if (_target == null || !_target.IsAvailable()) {
            return;
        }
        _target.TakeDamage(Data.Damage);
        AudioSource.PlayClipAtPoint(AttackSound, transform.position);
    }
}
