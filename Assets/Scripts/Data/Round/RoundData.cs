using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundData", menuName = "Necromancer/Round/Data", order = 1)]
public class RoundData : ScriptableObject {

    [SerializeField]
    private bool Melee = true;
    [ShowIf("Melee")]
    [SerializeField]
    private int MeleesCount;
    [ShowIf("Melee")]

    [SerializeField]
    private bool Ranged = true;
    [ShowIf("Ranged")]
    [SerializeField]
    private int ArchersCount;

    [SerializeField]
    private bool Civilians = true;
    [ShowIf("Civilians")]
    [SerializeField]
    private int CiviliansCount;

    public Guid UUID { get; private set; }

    public Lazy<List<Zombifiable>> CiviliansPrefabs {

    }
    public Lazy<List<Defender>> Defenders {
        get {
            Lazy<List<Defender>> defenders = new();
            if (Melee) {
                for (int i = 0; i < MeleesCount; i++) {
                    Defender melee = CharactersManager.Instance.GetDefenderPrefab(DefenderType.Melee);
                    defenders.Value.Add(melee);
                }
            }
            if (Ranged) {
                for (int i = 0; i < ArchersCount; i++) {
                    Defender archer = CharactersManager.Instance.GetDefenderPrefab(DefenderType.Ranged, DefenderRangedType.Archer);
                    defenders.Value.Add(archer);
                }
            }
            return defenders;
        }
    }
    public int Reward;

    [Header("Sound")]
    public AudioClip BackgroundMusic;

    public RoundData() => UUID = Guid.NewGuid();
}