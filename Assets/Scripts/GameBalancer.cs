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
            damage: 11,
            health: 100,
            speed: 2.5f,
            attackSpeed: 2
        );
        // Placeholder for ranged defender stats
        rangedDefenderStats = new();
        // Base stats for zombies at level 1
        zombieBaseStats = new Dictionary<ZombieType, ZombieLevel> {
            { ZombieType.Small, new(
            level: 1,
            damage: 10,
            health: 50,
            speed: 2.5f,
            attackSpeed: 2,
            amountSpawned: 3,
            priceToUse: 0,
            priceToUpgrade: 0,
            detectionRange: 3
            )},
            { ZombieType.Medium, new(level: 1,
            damage: 25,
            health: 100,
            speed: 2.5f,
            attackSpeed: 2,
            amountSpawned: 1,
            priceToUse: 50,
            priceToUpgrade: 150,
            detectionRange: 1
            ) },
            { ZombieType.Large, new(
            level: 1,
            damage: 100,
            health: 300,
            speed: 2.5f,
            attackSpeed: 0.5f,
            amountSpawned: 1,
            priceToUse: 150,
            priceToUpgrade: 300,
            detectionRange: 1
            )
            }
        };
        zombieStats = new Dictionary<ZombieType, Dictionary<int, ZombieLevel>>();

        CalculateZombieStats();
        CalculateRangedDefenderStats();
    }

    private void CalculateZombieStats()
    {
        foreach (ZombieType type in Enum.GetValues(typeof(ZombieType)))
        {
            if (type == ZombieType.Playground) continue;
            var baseStats = zombieBaseStats[type];
            zombieStats[type] = new Dictionary<int, ZombieLevel>();
            for (int level = 1; level <= 12; level++)
            {
                // Linear progression calculation
                float factor = (level - 1) / 11f;
                zombieStats[type][level] = new(
                   level: level,
                   damage: (int)(baseStats.Damage * (1 + factor)),
                   health: (int)(baseStats.Health * (1 + factor)),
                   speed: baseStats.Speed * (1 + factor),
                   attackSpeed: baseStats.AttackSpeed * (1 + factor),
                   amountSpawned: baseStats.AmountSpawned + (int)(level / 3),
                   priceToUse: baseStats.PriceToUse,
                   priceToUpgrade: baseStats.PriceToUpgrade + (50 * (level - 1)),
                   detectionRange: baseStats.DetectionRange
                );
            }
        }
    }

    private void CalculateRangedDefenderStats()
    {
        // Using the condition: 1.5 small zombies (level 1) = 1 ranged defender
        var smallZombieStats = zombieBaseStats[ZombieType.Small];
        float totalZombiePower =
            1.5f * (smallZombieStats.Health + smallZombieStats.Damage + smallZombieStats.AttackSpeed);
        float zombieDps = smallZombieStats.Damage * smallZombieStats.AttackSpeed;
        int zombieTimeToKillRanged = 3;
        int rangedHealth = (int)(zombieDps * 1.5 / zombieTimeToKillRanged);
        float rangedDps = smallZombieStats.Health / 2;
        float rangedAttackSpeed = 2;
        float rangedDamage = rangedDps * rangedAttackSpeed;
        rangedDefenderStats = new DefenderData(
            level: 1,
            damage: (int)rangedDamage,
            health: rangedHealth,
            speed: 2.5f,
            attackSpeed: rangedAttackSpeed
        );
    }

    // Methods to access the stats (optional)
    public DefenderData GetMeleeDefenderStats()
    {
        return meleeDefenderStats;
    }

    public DefenderData GetRangedDefenderStats()
    {
        return rangedDefenderStats;
    }

    public Dictionary<int, ZombieLevel> GetZombieStats(ZombieType type)
    {
        return zombieStats[type];
    }
}