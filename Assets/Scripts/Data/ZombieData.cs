using UnityEngine;

public static class ZombieData
{
    public static int MaxLevel => 12;

    public static ZombieLevel GetLevel(int level, ZombieType type)
    {
        int clampedLevel = Mathf.Clamp(level, 0, MaxLevel);
        return GameBalancer.Instance.GetZombieStats(type)[clampedLevel];
    }

    public static int GetPriceToAcquire(ZombieType type)
    {
        return GameBalancer.Instance.GetZombieStats(type)[1].PriceToAcquire;
    }
}
