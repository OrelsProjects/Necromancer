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
    public int Speed; // The movement speed of the defender
    [Tooltip("The attack speed of the defender")]
    public int AttackSpeed; // The attack speed of the defender
    [Tooltip("The range from which to start attacking")]
    public float AttackRange = 0.5f; // The range from which to start attacking

    public DefenderData()
    {
        Level = 1;
        Damage = 1;
        Health = 1;
        Speed = 1;
        AttackSpeed = 1;
    }
}
