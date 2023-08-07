using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Necromancer/Zombie/Level", order = 1)]
public class ZombieLevel : ScriptableObject
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
    [Tooltip("How many zombies will be spawned")]
    public int AmountSpawned; // How many zombies will be spawned
    [Tooltip("Price to include in a raid")]
    public int PriceToUse; // Price to include in a raid
    [Tooltip("Price to upgrade to this level")]
    public int PriceToUpgrade; // Price to upgrade to this level

    public ZombieLevel()
    {
        Level = 1;
        Damage = 1;
        Health = 1;
        Speed = 1;
        AttackSpeed = 1;
        AmountSpawned = 1;
        PriceToUse = 1;
        PriceToUpgrade = 1;
    }
}
