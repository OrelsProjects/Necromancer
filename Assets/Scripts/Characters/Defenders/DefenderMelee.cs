using UnityEngine;

public class DefenderMelee : Defender
{
    [Header("Sounds")]
    [SerializeField]
    private AudioClip _attackSound;

    [Header("Stats")]
    [SerializeField]
    private int _health = 100;
    [SerializeField]
    private int _damage = 20;
    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private float _attackSpeed = 1;


    public override AudioClip AttackSound
    {
        get { return _attackSound; }
        set { _attackSound = value; }
    }
    public override int Health
    {
        get { return _health; }
        set { _health = value; }
    }

    public override int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    public override float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public override float AttackSpeed
    {
        get { return _attackSpeed; }
        set { _attackSpeed = value; }
    }
    
    public override void Attack()
    {
        _animationHelper.PlayAnimation(AnimationType.AttackMelee);
    }
}
