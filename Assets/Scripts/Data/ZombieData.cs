using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Necromancer/Zombie/Data", order = 2)]
public class ZombieData : ScriptableObject
{
    [SerializeField]
    private ZombieLevel[] _levels;
    [SerializeField]
    private int _currentLevel;

    public void SetCurrentLevel(int level)
    {
        _currentLevel = level;
    }

    private ZombieLevel GetLevel(int level)
    {
        if (_levels != null)
        {
            return _levels.FirstOrDefault(x => x.Level == level);
        }
        else
        {
            return new();
        }
    }

    public ZombieLevel GetCurrentLevel()
    {
        return GetLevel(_currentLevel);
    }

    public ZombieLevel GetNextLevel()
    {
        return GetLevel(_currentLevel + 1);
    }
}
