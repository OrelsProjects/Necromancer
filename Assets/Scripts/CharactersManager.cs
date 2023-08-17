using System.Collections.Generic;
using UnityEngine;

public enum ZombieType {
    ZombieLab,
    Playground
}

public struct CharacterData : ISaveableObject {
    public int BasicZombieLevel;

    public string GetObjectType() {
        return GetType().FullName;
    }
}

public class CharactersManager : MonoBehaviour, ISaveable {
    public static CharactersManager Instance;

    // TODO: Remove sprite from here.
    [SerializeField]
    private Sprite _basicZombieSprite;
    [SerializeField]
    private ZombieData _basicZombieData;
    [Header("Prefabs")]
    [SerializeField]
    private Zombie _basicZombiePrefab;
    [SerializeField]
    private Zombie _playgroundZombiePrefab;
    [SerializeField]
    private Defender _meleeDefenderPrefab;
    [SerializeField]
    private Defender _archerDefenderPrefab;
    [SerializeField]
    private List<Civilian> _civilianPrefabs = new();

    private int _basicZombieLevel = 1;

    public int BasicZombieLevel {
        get { return _basicZombieLevel; }
        private set {
            if (_basicZombieLevel != value) {
                if (value <= 0) {
                    _basicZombieLevel = 1;
                } else {
                    _basicZombieLevel = value;
                }
                SaveManager.Instance.InitiateSave();
            }
        }
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            return;
        }
    }

    public ZombieLevel GetZombieData(ZombieType type) {
        return type switch {
            ZombieType.ZombieLab => _basicZombieData.GetLevel(BasicZombieLevel),
            ZombieType.Playground => _basicZombieData.GetLevel(BasicZombieLevel),
            _ => new(),
        };
    }

    public bool IsZombieMaxLevel(ZombieType type) {
        return type switch {
            ZombieType.ZombieLab => BasicZombieLevel >= _basicZombieData.MaxLevel,
            ZombieType.Playground => BasicZombieLevel >= _basicZombieData.MaxLevel,
            _ => false,
        };
    }

    public ZombieLevel GetZombieDataNextLevel(ZombieType type) {
        return type switch {
            ZombieType.ZombieLab => _basicZombieData.GetLevel(BasicZombieLevel + 1),
            ZombieType.Playground => _basicZombieData.GetLevel(BasicZombieLevel + 1),
            _ => new(),
        };
    }

    public Zombie GetZombiePrefab(ZombieType type) {
        return type switch {
            ZombieType.ZombieLab => _basicZombiePrefab,
            ZombieType.Playground => _playgroundZombiePrefab,
            _ => null,
        };
    }

    public Defender GetDefenderPrefab(DefenderType type, DefenderRangedType rangedType = default) {
        return type switch {
            DefenderType.Melee => _meleeDefenderPrefab,
            DefenderType.Ranged => rangedType switch {
                DefenderRangedType.Archer => _archerDefenderPrefab,
                _ => null,
            },
            _ => null,
        };
    }

    public Civilian GetRandomCivlian() => _civilianPrefabs[Random.Range(0, _civilianPrefabs.Count - 1)];

    public Sprite GetZombieSprite(ZombieType type) => type switch {
        ZombieType.ZombieLab => _basicZombieSprite,
        ZombieType.Playground => _basicZombieSprite,
        _ => null,
    };

    public void LoadData(ISaveableObject item) {
        if (item is CharacterData data) {
            BasicZombieLevel = data.BasicZombieLevel;
        } else {
            BasicZombieLevel = 1;
        }
    }

    public ISaveableObject GetData() =>
        new CharacterData {
            BasicZombieLevel = BasicZombieLevel,
        };


    public void UpgradeZombie(ZombieType type) {
        switch (type) {
            case ZombieType.ZombieLab:
                BasicZombieLevel++;
                break;
        }
    }
}