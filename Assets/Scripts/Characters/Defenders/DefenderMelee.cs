using UnityEngine;

public class DefenderMelee : Defender
{
    [Header("Sounds")]
    [SerializeField]
    private AudioClip _attackSound;

    public override AudioClip AttackSound
    {
        get { return _attackSound; }
        set { _attackSound = value; }
    }
    
    public override void Attack()
    {
        _animationHelper.PlayAnimation(AnimationType.AttackMelee);
    }
}
