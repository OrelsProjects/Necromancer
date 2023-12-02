using UnityEngine;

public class ZombieLevel
{
    public int Level; // The level of the zombie
    public int Damage; // The damage inflicted by the zombie
    public int Health; // The health points of the zombie
    public float Speed; // The movement speed of the zombie
    public float AttackSpeed; // The attack speed of the zombie
    public int AmountSpawned; // How many zombies will be spawned
    [Tooltip("Price to include in a raid")]
    public int PriceToUse; // Price to include in a raid
    public int PriceToUpgrade; // Price to upgrade to this level
    public int PriceToAcquire; // Price to acquire this zombie
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
        float detectionRange = 1,
        int priceToAcquire = 0
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
        PriceToAcquire = priceToAcquire;
    }
}
