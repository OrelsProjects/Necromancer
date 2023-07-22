using UnityEngine;

public class DefenderMelee : Defender
{
    [SerializeField]
    private int _health = 100;
    [SerializeField]
    private int _damage = 20;
    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private float _attackSpeed = 1;


    public override int Health { get; set; }
    public override int Damage { get; set; }
    public override float Speed { get; set; }
    public override float AttackSpeed { get; set; }


    public override void Start()
    {
        base.Start();
        Health = _health;
        Damage = _damage;
        Speed = _speed;
        AttackSpeed = _attackSpeed;
    }

    public override void Attack()
    {
        _animationHelper.PlayAnimation(AnimationType.AttackMelee);
    }
}
