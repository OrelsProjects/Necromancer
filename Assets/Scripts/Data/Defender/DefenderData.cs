using UnityEngine;

[CreateAssetMenu(fileName = "Zombie Data", menuName = "Necromancer/Defender/Data", order = 1)]
public class DefenderData : ScriptableObject
{
    [Tooltip("The level of the defender")]
    public int Level; // The level of the defender
    [Tooltip("The damage inflicted by the defender")]
    public int Damage; // The damage inflicted by the defender
    [Tooltip("The health points of the defender")]
    public int Health; // The health points of the defender
    [Tooltip("The movement speed of the defender")]
    public float Speed; // The movement speed of the defender
    [Tooltip("The attack speed of the defender")]
    public float AttackSpeed; // The attack speed of the defender
    [Tooltip("The range from which to start attacking")]
    public float AttackRange = 0.5f; // The range from which to start attacking

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
    }
}
