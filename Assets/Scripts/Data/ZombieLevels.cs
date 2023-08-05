using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsData", menuName = "Necromancer/Zombie/Levels", order = 2)]
public class ZombieLevels : ScriptableObject
{
    [SerializeField]
    private ZombieLevelDTO[] _levels;

    public ZombieLevelDTO[] LevelsArray
    {
        get => _levels;
        set => _levels = value;
    }
}
