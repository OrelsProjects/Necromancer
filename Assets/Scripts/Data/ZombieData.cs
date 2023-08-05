using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Zombie Data", menuName = "Necromancer/Zombie/Data", order = 2)]
public class ZombieData : ScriptableObject
{
    [SerializeField]
    private ZombieLevels _levels;

    public ZombieLevelDTO GetLevel(int level)
    {
        if (level < 0 || level >= _levels.LevelsArray.Length)
        {
            return null;
        }
        return _levels.LevelsArray[level];
    }
}
