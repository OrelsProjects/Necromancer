using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Necromancer/Pod/Data", order = 0)]
public class PodData : ScriptableObject
{
    [SerializeField]
    private List<PodLevel> _podLevels;

    private Dictionary<int, PodLevel> _levels
    {
        get
        {
            Dictionary<int, PodLevel> levels = new();
            _podLevels.ForEach(podLevel => levels.TryAdd(podLevel.Level, podLevel));
            return levels;
        }
    }

    public int MaxLevel
    {
        get { return _levels.Keys.Count; }
    }

    public PodLevel GetLevel(int level)
    {
        int clampedLevel = Mathf.Clamp(level, 0, MaxLevel);
        return _levels[clampedLevel];
    }
}