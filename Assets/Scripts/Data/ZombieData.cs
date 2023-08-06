using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Zombie Data", menuName = "Necromancer/Zombie/Data", order = 2)]
public class ZombieData : ScriptableObject
{
    [SerializeField]
    private ZombieLevels _levels;
    [SerializeField]
    private int _currentLevel;

    public ZombieData(int level)
    {
        _currentLevel = level;
    }

    private ZombieLevelDTO GetLevel(int level)
    {
        if (level < 0 || level >= _levels.LevelsArray.Length)
        {
            return null;
        }
        return _levels.LevelsArray[level];
    }

    public ZombieLevelDTO GetCurrentLevel()
    {
        return GetLevel(_currentLevel);
    }

    public ZombieLevelDTO GetNextLevel()
    {
        return GetLevel(_currentLevel + 1);
    }
}
