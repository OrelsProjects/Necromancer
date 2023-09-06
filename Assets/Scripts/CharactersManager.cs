using System.Collections.Generic;
using UnityEngine;

public enum ZombieType {
    ZombieLab,
    ZombieLab1,
    Playground
}

public struct CharacterData : ISaveableObject {
    public int BasicZombieLevel;
    public int BasicZombieLevel1;

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
    [SerializeField]
    private Sprite _basicZombieSprite1;
    [SerializeField]
    private ZombieData _basicZombieData1;
    [Header("Prefabs")]
    [SerializeField]
    private Zombie _basicZombiePrefab;
    [SerializeField]
    private Zombie _basicZombiePrefab1;
    [SerializeField]
    private Zombie _playgroundZombiePrefab;
    [SerializeField]
    private Defender _meleeDefenderPrefab;
    [SerializeField]
    private Defender _archerDefenderPrefab;
    [SerializeField]
    private List<Civilian> _civilianPrefabs = new();

    private int _basicZombieLevel = 1;
    private int _basicZombieLevel1 = 1;

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

    public int BasicZombieLevel1 {
        get { return _basicZombieLevel1; }
        private set {
            if (_basicZombieLevel1 != value) {
                if (value <= 0) {
                    _basicZombieLevel1 = 1;
                } else {
                    _basicZombieLevel1 = value;
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
            ZombieType.ZombieLab1 => _basicZombieData1.GetLevel(BasicZombieLevel1),
            ZombieType.Playground => _basicZombieData.GetLevel(BasicZombieLevel),
            _ => new(),
        };
    }

    public bool IsZombieMaxLevel(ZombieType type) {
        return type switch {
            ZombieType.ZombieLab => BasicZombieLevel >= _basicZombieData.MaxLevel,
            ZombieType.ZombieLab1 => BasicZombieLevel1 >= _basicZombieData1.MaxLevel,
            ZombieType.Playground => BasicZombieLevel >= _basicZombieData.MaxLevel,
            _ => false,
        };
    }

    public ZombieLevel GetZombieDataNextLevel(ZombieType type) {
        return type switch {
            ZombieType.ZombieLab => _basicZombieData.GetLevel(BasicZombieLevel + 1),
            ZombieType.ZombieLab1 => _basicZombieData1.GetLevel(BasicZombieLevel + 1),
            ZombieType.Playground => _basicZombieData.GetLevel(BasicZombieLevel + 1),
            _ => new(),
        };
    }

    public Zombie GetZombiePrefab(ZombieType type) {
        return type switch {
            ZombieType.ZombieLab => _basicZombiePrefab,
            ZombieType.ZombieLab1 => _basicZombiePrefab1,
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

    public Civilian GetRandomCivlian() => _civilianPrefabs[Random.Range(0, _civilianPrefabs.Count)];

    public Sprite GetZombieSprite(ZombieType type) => type switch {
        ZombieType.ZombieLab => _basicZombieSprite,
        ZombieType.ZombieLab1 => _basicZombieSprite1,
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
            BasicZombieLevel1 = BasicZombieLevel1
        };


    public void UpgradeZombie(ZombieType type) {
        switch (type) {
            case ZombieType.ZombieLab:
                BasicZombieLevel++;
                break;
            case ZombieType.ZombieLab1:
                BasicZombieLevel1++;
                break;
        }
    }
}