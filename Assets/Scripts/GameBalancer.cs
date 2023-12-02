using System;
using System.Collections.Generic;

public struct CharacterStatus
{
    public float Hp;
    public float Damage;
    public float AttackSpeed;

    public CharacterStatus(float hp, float damage, float attackSpeed)
    {
        Hp = hp;
        Damage = damage;
        AttackSpeed = attackSpeed;
    }
}

public class GameBalancer
{
    private static GameBalancer _instance;

    public static GameBalancer Instance
    {
        get
        {
            _instance ??= new GameBalancer();
            return _instance;
        }
    }
    private DefenderData meleeDefenderStats;
    private DefenderData rangedDefenderStats;
    private Dictionary<ZombieType, ZombieLevel> zombieBaseStats;
    private Dictionary<ZombieType, Dictionary<int, ZombieLevel>> zombieStats;

    public Dictionary<ZombieType, Dictionary<int, ZombieLevel>> ZombieStats
    {
        private set { zombieStats = value; }
        get { return zombieStats; }
    }

    private GameBalancer()
    {
        meleeDefenderStats = new(
            level: 1,
            damage: 40,
            health: 100,
            speed: 2.5f,
            attackSpeed: 2
        );
        // Placeholder for ranged defender stats
        rangedDefenderStats = new();
        // Base stats for zombies at level 1
        zombieBaseStats = new Dictionary<ZombieType, ZombieLevel> {
            { ZombieType.Small, CreateZombieLevel(
            level: 1,
            damage: 20,
            health: 75,
            speed: 2.5f,
            attackSpeed: 2.4f,
            amountSpawned: 3,
            priceToUse: 0,
            priceToUpgrade: 0,
            detectionRange: 3,
            priceToAcquire: 0
            )},
            { ZombieType.Medium, new(level: 1,
            damage: 76,
            health: 180,
            speed: 2.5f,
            attackSpeed: 2,
            amountSpawned: 1,
            priceToUse: 10,
            priceToUpgrade: 500,
            detectionRange: 1,
            priceToAcquire: 600
            ) },
            { ZombieType.Large, new(
            level: 1,
            damage: 297,
            health: 700,
            speed: 2.5f,
            attackSpeed: 1f,
            amountSpawned: 1,
            priceToUse: 100,
            priceToUpgrade: 2000,
            detectionRange: 1,
            priceToAcquire: 1000
            )
            }
        };
        ZombieStats = new Dictionary<ZombieType, Dictionary<int, ZombieLevel>>();

        InitZombieStats();
        InitRangedDefenderStats();
    }

    private void InitZombieStats()
    {
        foreach (ZombieType type in Enum.GetValues(typeof(ZombieType)))
        {
            if (type == ZombieType.Playground) continue;
            var baseStats = zombieBaseStats[type];
            ZombieStats[type] = new Dictionary<int, ZombieLevel>();
            for (int level = 1; level <= 12; level++)
            {
                // Linear progression calculation
                float factor = (level - 1) / 11f;
                ZombieStats[type][level] = new(
                     level: level,
                     damage: (int)(baseStats.Damage * (1 + factor)),
                     health: (int)(baseStats.Health * (1 + factor)),
                     speed: baseStats.Speed * (1 + factor),
                     attackSpeed: baseStats.AttackSpeed * (1 + factor),
                     amountSpawned: baseStats.AmountSpawned + level / 3,
                     priceToUse: baseStats.PriceToUse + (50 * (level - 1)),
                     priceToUpgrade: baseStats.PriceToUpgrade + (50 * level),
                     detectionRange: baseStats.DetectionRange,
                        priceToAcquire: baseStats.PriceToAcquire
                  );
            }
        }
    }

    private void InitRangedDefenderStats()
    {
        // Using the condition: 1.5 small zombies (level 1) = 1 ranged defender
        var smallZombieStats = zombieBaseStats[ZombieType.Small];
        float zombieDps = smallZombieStats.Damage * smallZombieStats.AttackSpeed;
        int zombieTimeToKillRanged = 3;
        int rangedHealth = (int)(zombieDps * zombieTimeToKillRanged);
        float rangedAttackSpeed = 1.4f;
        int rangedDamage = smallZombieStats.Health / 4;
        rangedDefenderStats = new(
            level: 1,
            damage: rangedDamage,
            health: rangedHealth,
            speed: 1.5f,
            attackSpeed: rangedAttackSpeed,
            attackRange: 6f
        );
    }

    private ZombieLevel CreateZombieLevel(
        int level,
        int damage,
        int health,
        float speed,
        float attackSpeed,
        int amountSpawned,
        int priceToUse,
        int priceToUpgrade,
        float detectionRange,
        int priceToAcquire
    ) => new(
        level: level,
        damage: damage,
        health: health,
        speed: speed,
        attackSpeed: attackSpeed,
        amountSpawned: amountSpawned,
        priceToUse: priceToUse,
        priceToUpgrade: priceToUpgrade,
        detectionRange: detectionRange,
        priceToAcquire: priceToAcquire
);
    public DefenderData GetMeleeDefenderStats(bool empowered = false)
    {
        if (empowered)
        {
            meleeDefenderStats.Empower();
        }
        return meleeDefenderStats;
    }

    public DefenderData GetRangedDefenderStats(bool empowered = false)
    {
        if (empowered)
        {
            rangedDefenderStats.Empower();
        }
        return rangedDefenderStats;
    }

    public Dictionary<int, ZombieLevel> GetZombieStats(ZombieType type)
    {
        return ZombieStats[type];
    }

    public int GetZombiePriceToAcquire(ZombieType type)
    {
        return zombieBaseStats[type].PriceToUse;
    }
}