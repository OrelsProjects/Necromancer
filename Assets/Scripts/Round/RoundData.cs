using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundData", menuName = "Necromancer/RoundData", order = 1)]
public class RoundData : ScriptableObject
{
    public Guid UUID { get; private set; }
    
    public int CiviliansCount;
    public List<Zombifiable> CiviliansPrefabs;
    public List<Defender> Defenders;
    public int Reward;

    public RoundData() => UUID = Guid.NewGuid();
}