using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Necromancer/Zombie/Data", order = 2)]
public class ZombieData : ScriptableObject
{
    [SerializeField]
    private ZombieLevel[] _levels;

    public ZombieLevel GetLevel(int level)
    {
        if (_levels != null)
        {
            return _levels.FirstOrDefault(x => x.Level == level);
        }
        return _levels[^1];
    }
}
