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
    public float Speed; // The movement speed of the zombie
    [Tooltip("The attack speed of the zombie")]
    public float AttackSpeed; // The attack speed of the zombie
    [Tooltip("How many zombies will be spawned")]
    public int AmountSpawned; // How many zombies will be spawned
    [Tooltip("Price to include in a raid")]
    public int PriceToUse; // Price to include in a raid
    [Tooltip("Price to upgrade to this level")]
    public int PriceToUpgrade; // Price to upgrade to this level
    [Tooltip("The range of the zombie defenders detection")]
    public float DetectionRange;

    public ZombieLevel(
        int level = 1,
        int damage = 1,
        int health = 1,
        float speed = 1,
        float attackSpeed = 1,
        int amountSpawned = 1,
        int priceToUse = 1,
        int priceToUpgrade = 1,
        float detectionRange = 1
    )
    {
        Level = level;
        Damage = damage;
        Health = health;
        Speed = speed;
        AttackSpeed = attackSpeed;
        AmountSpawned = amountSpawned;
        PriceToUse = priceToUse;
        PriceToUpgrade = priceToUpgrade;
        DetectionRange = detectionRange;
    }
}
