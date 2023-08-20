using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Necromancer/Zombie/Data", order = 2)]
public class ZombieData : ScriptableObject
{
    [SerializeField]
    private List<ZombieLevel> _zombieLevels;

    private Dictionary<int, ZombieLevel> _levels
    {
        get
        {
            Dictionary<int, ZombieLevel> levels = new();
            _zombieLevels.ForEach(zombieLevel => levels.Add(zombieLevel.Level, zombieLevel));
            return levels;
        }
    }

    public int MaxLevel
    {
        get { return _levels.Keys.Count; }
    }

    public ZombieLevel GetLevel(int level)
    {
        int clampedLevel = Mathf.Clamp(level, 0, MaxLevel);
        return _levels[clampedLevel];
    }
}
