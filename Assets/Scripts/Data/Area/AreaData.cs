using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaData", menuName = "Necromancer/Area/Data", order = 1)]
public class AreaData : ScriptableObject
{
    [SerializeField]
    private List<AreaLevel> _areaLevels;

    public Areas Area;
    public RoundData RoundData;

    public int RaidCost => RoundData.RaidCost;

    public Dictionary<int, AreaLevel> Levels
    {
        get
        {
            Dictionary<int, AreaLevel> levels = new();
            _areaLevels.ForEach(areaLevel => levels.Add(areaLevel.Level, areaLevel));
            return levels;
        }
    }

    public string AreaNameString
    {
        get
        {
            return Area.ToString();
        }
    }

    public AreaLevel GetAreaLevel(int level)
    {
        Debug.Log("What??");
        int clampedLevel = Mathf.Clamp(level, 0, MaxLevel);
        return Levels[clampedLevel];
    }

    public int MaxLevel
    {
        get
        {
            return Levels.Count;
        }
    }
}