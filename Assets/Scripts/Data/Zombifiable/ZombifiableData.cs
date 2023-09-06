
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Necromancer/Zombifiable/Data", order = 1)]
public class ZombifiableData : ScriptableObject
{
    public int Health = 3;
    public float MovementBlockedTimeAfterAttack = 1.2f;
    [Range(0, 1)]
    public float ChanceToBecomeZombie = 0.1f;
}
