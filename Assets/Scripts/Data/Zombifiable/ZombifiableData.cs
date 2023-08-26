
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Necromancer/Zombifiable/Data", order = 1)]
public class ZombifiableData : ScriptableObject
{
    public int HitsToZombify = 3;
    public float MovementBlockedTimeAfterAttack = 1.2f;
    public float TurnChance = 0.1f;

    [Header("Sounds")]
    public AudioClip DeathSound;
}
