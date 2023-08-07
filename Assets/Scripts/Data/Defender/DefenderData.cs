using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Zombie Data", menuName = "Necromancer/Defender/Data", order = 1)]
public class DefenderData : ScriptableObject
{
    [Tooltip("The level of the zombie")]
    public int Level; // The level of the zombie
    [Tooltip("The damage inflicted by the zombie")]
    public int Damage; // The damage inflicted by the zombie
    [Tooltip("The health points of the zombie")]
    public int Health; // The health points of the zombie
    [Tooltip("The movement speed of the zombie")]
    public int Speed; // The movement speed of the zombie
    [Tooltip("The attack speed of the zombie")]
    public int AttackSpeed; // The attack speed of the zombie

    public DefenderData()
    {
        Level = 1;
        Damage = 1;
        Health = 1;
        Speed = 1;
        AttackSpeed = 1;
    }
}
