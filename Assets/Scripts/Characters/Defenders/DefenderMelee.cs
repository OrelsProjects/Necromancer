using UnityEngine;

public class DefenderMelee : Defender
{
    [Header("Melee Defender Stats")]
    [Header("Sounds")]
    [SerializeField]
    private AudioClip _attackSound;

    public override AudioClip AttackSound
    {
        get { return _attackSound; }
        set { _attackSound = value; }
    }

    public override void Attack(Transform target)
    {
        _animationHelper.PlayAnimation(AnimationType.AttackMelee);
    }
}
