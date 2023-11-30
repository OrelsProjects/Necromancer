using System;
using System.Collections.Generic;

public struct ZombieStats
{
    public float Hp;
    public float Damage;
    public float AttackSpeed;

    public ZombieStats(float hp, float damage, float attackSpeed)
    {
        Hp = hp;
        Damage = damage;
        AttackSpeed = attackSpeed;
    }
}

public class GameBalancer
{
    private ZombieStats meleeDefenderStats;
    private ZombieStats rangedDefenderStats;
    private Dictionary<ZombieType, ZombieStats> zombieBaseStats;
    private Dictionary<ZombieType, Dictionary<int, ZombieStats>> zombieStats;

    public GameBalancer()
    {
        meleeDefenderStats = new ZombieStats(100, 11, 2);
        // Placeholder for ranged defender stats
        rangedDefenderStats = new ZombieStats(0, 0, 0);
        // Base stats for zombies at level 1
        zombieBaseStats = new Dictionary<ZombieType, ZombieStats> {
            { ZombieType.Small, new ZombieStats(50, 10, 2) },
            { ZombieType.Medium, new ZombieStats(100, 25, 1) },
            { ZombieType.Large, new ZombieStats(300, 100, 0.5f) }
        };
        zombieStats = new Dictionary<ZombieType, Dictionary<int, ZombieStats>>();

        CalculateZombieStats();
        CalculateRangedDefenderStats();
    }

    private void CalculateZombieStats()
    {
        foreach (ZombieType type in Enum.GetValues(typeof(ZombieType)))
        {
            if (type == ZombieType.Playground) continue;
            var baseStats = zombieBaseStats[type];
            zombieStats[type] = new Dictionary<int, ZombieStats>();
            for (int level = 1; level <= 12; level++)
            {
                // Linear progression calculation
                float factor = (level - 1) / 11f; // Normalized level factor (0 at level 1, 1 at level 12)
                zombieStats[type][level] = new ZombieStats(
                    baseStats.Hp * (1 + factor),
                    baseStats.Damage * (1 + factor),
                    baseStats.AttackSpeed * (1 + factor)
                );
            }
        }
    }

    private void CalculateRangedDefenderStats()
    {
        // Using the condition: 1.5 small zombies (level 1) = 1 ranged defender
        var smallZombieStats = zombieBaseStats[ZombieType.Small];
        float totalZombiePower =
            1.5f * (smallZombieStats.Hp + smallZombieStats.Damage + smallZombieStats.AttackSpeed);
        // Assuming equal distribution of total power among hp, damage, and attack speed
        float statValue = totalZombiePower / 3;
        rangedDefenderStats = new ZombieStats(statValue, statValue, statValue);
    }

    // Methods to access the stats (optional)
    public ZombieStats GetMeleeDefenderStats()
    {
        return meleeDefenderStats;
    }

    public ZombieStats GetRangedDefenderStats()
    {
        return rangedDefenderStats;
    }

    public Dictionary<int, ZombieStats> GetZombieStats(ZombieType type)
    {
        return zombieStats[type];
    }
}

// Usage
public class ExampleUsage
{
    static void Main(string[] args)
    {
        GameBalancer gameBalancer = new GameBalancer();
        Console.WriteLine("Zombie Stats:");
        foreach (var type in gameBalancer.GetZombieStats(ZombieType.Small))
        {
            Console.WriteLine($"Level {type.Key}: HP={type.Value.Hp}, Damage={type.Value.Damage}, AttackSpeed={type.Value.AttackSpeed}");
        }
        var rangedStats = gameBalancer.GetRangedDefenderStats();
        Console.WriteLine($"Ranged Defender Stats: HP={rangedStats.Hp}, Damage={rangedStats.Damage}, AttackSpeed={rangedStats.AttackSpeed}");
    }
}
