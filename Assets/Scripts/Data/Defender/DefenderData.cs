
public class DefenderData
{
    private int _level; // The level of the defender
    private int _damage; // The damage inflicted by the defender
    private int _health; // The health points of the defender
    private float _speed; // The movement speed of the defender
    private float _attackSpeed; // The attack speed of the defender
    private float _attackRange = 0.5f; // The range from which to start attacking

    private readonly DefenderType _type;

    private bool _empower;

    public int Level
    {
        private set { _level = value; }
        get { return _level; }
    }

    public int Damage
    {
        private set { _damage = value; }
        get { return _damage * (_empower ? 2 : 1); }
    }

    public int Health
    {
        private set { _health = value; }
        get { return _health * (_empower ? 2 : 1); }
    }

    public float Speed
    {
        private set { _speed = value; }
        get { return _speed * (_empower ? 2 : 1); }
    }

    public float AttackSpeed
    {
        private set { _attackSpeed = value; }
        get { return _attackSpeed * (_empower ? 2 : 1); }
    }

    public float AttackRange
    {
        private set { _attackRange = value; }
        get { return _attackRange * (_empower ? 2 : 1); }
    }

    public void Empower()
    {
        _empower = true;
    }

    public DefenderData(
        int level = 1,
        int damage = 1,
        int health = 1,
        float speed = 1,
        float attackSpeed = 1,
        float attackRange = 0.5f,
        DefenderType type = DefenderType.Melee
    )
    {
        _level = level;
        _damage = damage;
        _health = health;
        _speed = speed;
        _attackSpeed = attackSpeed;
        _attackRange = attackRange;
        _type = type;
    }
}
