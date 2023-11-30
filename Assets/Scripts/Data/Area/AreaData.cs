using System;
using System.Collections.Generic;
using UnityEngine;

public enum AreaDifficulty
{
    Easy,
    Medium,
    Hard
}

public struct DefenderCount
{
    public int Melee;
    public int Ranged;

    public DefenderCount(int melee, int ranged)
    {
        Melee = melee;
        Ranged = ranged;
    }
}

[CreateAssetMenu(fileName = "AreaData", menuName = "Necromancer/Area/Data", order = 1)]
public class AreaData : ScriptableObject
{
    [SerializeField]
    private List<AreaLevel> _areaLevels;

    public AreaDifficulty Difficulty;
    public Areas Area;
    public RoundData RoundData;

    // Properties for balancing
    public DefenderCount DefendersPerRound { get; private set; }
    public int GoldRewardPerRound { get; private set; }
    public int LabLevelUpgradeCost { get; private set; }

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

    public string AreaNameString => Area.ToString();

    public AreaLevel GetAreaLevel(int level)
    {
        int clampedLevel = Mathf.Clamp(level, 0, MaxLevel);
        return Levels[clampedLevel];
    }

    public int MaxLevel => Levels.Count;

    public void CalculateBalancingFactors(int round)
    {
        int baseMeleeCount = 2;
        int baseRangedCount = 1;
        int baseGoldReward = 100;
        int baseLabUpgradeCost = 500;

        int meleeMultiplier = round + (int)Difficulty;
        int rangedMultiplier = round + (int)Difficulty / 2;
        int goldMultiplier = round * (int)Difficulty;
        int labCostMultiplier = (int)Difficulty * 2;

        DefendersPerRound = new DefenderCount(
            baseMeleeCount * meleeMultiplier,
            baseRangedCount * rangedMultiplier
        );

        GoldRewardPerRound = baseGoldReward + goldMultiplier * 50;
        LabLevelUpgradeCost = baseLabUpgradeCost + round * labCostMultiplier * 100;
    }
}

[CreateAssetMenu(fileName = "AreaLevel", menuName = "Necromancer/Area/Level", order = 2)]
public class AreaLevel : ScriptableObject
{
    public int Level = 1;
    public int CurrencyPerMinute = 1;
    public int PriceToUpgrade = 1;
}
