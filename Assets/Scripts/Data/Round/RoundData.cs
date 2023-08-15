using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundData", menuName = "Necromancer/Round/Data", order = 1)]
public class RoundData : ScriptableObject {
    [Header("Characters")]
    [SerializeField]
    private bool Melee = true;
    [ShowIf("Melee")]
    [SerializeField]
    private int _meleesCount;

    [SerializeField]
    private bool Ranged = true;
    [ShowIf("Ranged")]
    [SerializeField]
    private int _archersCount;

    [SerializeField]
    private bool Civilians = true;
    [ShowIf("Civilians")]
    [SerializeField]
    private int _civiliansCount;

    public int CiviliansCount => _civiliansCount;
    public int ArchersCount => _archersCount;
    public int MeleesCount => _meleesCount;

    public Guid UUID { get; private set; }

    public Lazy<List<Zombifiable>> CiviliansPrefabs {
        get {
            Lazy<List<Zombifiable>> civilians = new();
            if (Civilians) {
                for (int i = 0; i < _civiliansCount; i++) {
                    Civilian civilian = CharactersManager.Instance.GetRandomCivlian();
                    Zombifiable zombifiable = civilian.GetComponent<Zombifiable>();

                    if (zombifiable != null) {
                        civilians.Value.Add(zombifiable);
                    }
                }
            }
            return civilians;
        }
    }
    public Lazy<List<Defender>> Defenders {
        get {
            Lazy<List<Defender>> defenders = new();
            if (Melee) {
                for (int i = 0; i < _meleesCount; i++) {
                    Defender melee = CharactersManager.Instance.GetDefenderPrefab(DefenderType.Melee);
                    defenders.Value.Add(melee);
                }
            }
            if (Ranged) {
                for (int i = 0; i < _archersCount; i++) {
                    Defender archer = CharactersManager.Instance.GetDefenderPrefab(DefenderType.Ranged, DefenderRangedType.Archer);
                    defenders.Value.Add(archer);
                }
            }
            return defenders;
        }
    }

    [Header("Reward")]
    public int Reward;

    [Header("Sound")]
    public AudioClip BackgroundMusic;

    public RoundData() => UUID = Guid.NewGuid();
}