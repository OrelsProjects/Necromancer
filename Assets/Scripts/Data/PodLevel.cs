using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct SpawnRate
{
    public int count;
    public float time;
}

[CreateAssetMenu(fileName = "Data", menuName = "Necromancer/Pod/Level", order = 0)]
public class PodLevel : ScriptableObject
{
    [SerializeField]
    private int _level;
    [SerializeField]
    private int _health;
    [SerializeField]
    private int _priceToUpgrade;
    [SerializeField]
    private ZombieType _zombieType;
    [Tooltip("How many zombies to spawn per unit of time")]
    [SerializeField]
    private SpawnRate _spawnRate;

    public int Level
    {
        get { return _level; }
    }

    public int Health
    {
        get { return _health; }
    }

    public int PriceToUpgrade
    {
        get { return _priceToUpgrade; }
    }

    public ZombieType ZombieType
    {
        get { return _zombieType; }
    }

    public SpawnRate SpawnRate
    {
        get { return _spawnRate; }
    }
}