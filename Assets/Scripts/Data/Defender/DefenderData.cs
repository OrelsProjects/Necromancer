using UnityEngine;

[CreateAssetMenu(fileName = "Zombie Data", menuName = "Necromancer/Defender/Data", order = 1)]
public class DefenderData : ScriptableObject
{
    [SerializeField]
    [Tooltip("The level of the defender")]
    private int _level; // The level of the defender
    [SerializeField]
    [Tooltip("The damage inflicted by the defender")]
    private int _damage; // The damage inflicted by the defender
    [SerializeField]
    [Tooltip("The health points of the defender")]
    private int _health; // The health points of the defender
    [SerializeField]
    [Tooltip("The movement speed of the defender")]
    private float _speed; // The movement speed of the defender
    [SerializeField]
    [Tooltip("The attack speed of the defender")]
    private float _attackSpeed; // The attack speed of the defender
    [SerializeField]
    [Tooltip("The range from which to start attacking")]
    private float _attackRange = 0.5f; // The range from which to start attacking
    [SerializeField]
    [Tooltip("The type of the defender")]
    private DefenderType _type;

    private bool _empower;
    private DefenderData _defenderData;

    public int Level
    {
        private set { _level = value; }
        get { return _level; }
    }

    public int Damage
    {
        private set { _damage = value; }
        get { return _defenderData.Damage * (_empower ? 2 : 1); }
    }

    public int Health
    {
        private set { _health = value; }
        get { return _defenderData.Health * (_empower ? 2 : 1); }
    }

    public float Speed
    {
        private set { _speed = value; }
        get { return _defenderData.Speed * (_empower ? 2 : 1); }
    }

    public float AttackSpeed
    {
        private set { _attackSpeed = value; }
        get { return _defenderData.AttackSpeed * (_empower ? 2 : 1); }
    }

    public float AttackRange
    {
        private set { _attackRange = value; }
        get { return _defenderData.AttackRange * (_empower ? 2 : 1); }
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
        float attackSpeed = 1
    )
    {
        Level = level;
        Damage = damage;
        Health = health;
        Speed = speed;
        AttackSpeed = attackSpeed;
        switch (_type)
        {
            case DefenderType.Melee:
                _defenderData = GameBalancer.Instance.GetMeleeDefenderStats();
                break;
            case DefenderType.Ranged:
                _defenderData = GameBalancer.Instance.GetRangedDefenderStats();
                break;
        }
    }
}
