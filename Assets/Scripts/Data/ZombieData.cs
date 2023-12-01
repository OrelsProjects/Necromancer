﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Necromancer/Zombie/Data", order = 2)]
public class ZombieData : ScriptableObject
{
    [SerializeField]
    private List<ZombieLevel> _zombieLevels;
    [SerializeField]
    private int _priceToAcquire;

    private Dictionary<int, ZombieLevel> levels
    {
        get
        {
            Dictionary<int, ZombieLevel> levels = new();
            _zombieLevels.ForEach(zombieLevel => levels.TryAdd(zombieLevel.Level, zombieLevel));
            return levels;
        }
    }

    public int MaxLevel
    {
        get { return levels.Keys.Count; }
    }

    public ZombieLevel GetLevel(int level, ZombieType type)
    {
        int clampedLevel = Mathf.Clamp(level, 0, MaxLevel);
        return GameBalancer.Instance.GetZombieStats(type)[clampedLevel];
    }

    public int PriceToAcquire => _priceToAcquire;
}
