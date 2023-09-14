using System.Collections.Generic;
using UnityEngine;

public enum ZombieType
{
    Small,
    Medium,
    Large,
    Playground
}

public struct CharacterData : ISaveableObject
{
    public int SmallZombieLevel;
    public int MediumZombieLevel;
    public int LargeZombieLevel;

    public string GetObjectType()
    {
        return GetType().FullName;
    }
}

public class CharactersManager : MonoBehaviour, ISaveable
{
    public static CharactersManager Instance;

    [SerializeField]
    private Sprite _smallZombieSprite;
    [SerializeField]
    private ZombieData _smallZombieData;
    [SerializeField]
    private Sprite _mediumZombieSprite;
    [SerializeField]
    private ZombieData _mediumZombieData;
    [SerializeField]
    private Sprite _largeZombieSprite;
    [SerializeField]
    private ZombieData _largeZombieData;

    [Header("Prefabs")]
    [SerializeField]
    private Zombie _smallZombiePrefab;
    [SerializeField]
    private Zombie _mediumZombiePrefab;
    [SerializeField]
    private Zombie _largeZombiePrefab;
    [SerializeField]
    private Zombie _playgroundZombiePrefab;
    [SerializeField]
    private Defender _meleeDefenderPrefab;
    [SerializeField]
    private Defender _archerDefenderPrefab;
    [SerializeField]
    private List<Civilian> _civilianPrefabs = new List<Civilian>();

    private int _smallZombieLevel = 1;
    private int _mediumZombieLevel = 1;
    private int _largeZombieLevel = 1;

    public int SmallZombieLevel
    {
        get { return _smallZombieLevel; }
        private set
        {
            if (_smallZombieLevel != value)
            {
                if (value <= 0)
                {
                    _smallZombieLevel = 1;
                }
                else
                {
                    _smallZombieLevel = value;
                }
                SaveManager.Instance.InitiateSave();
            }
        }
    }

    public int MediumZombieLevel
    {
        get { return _mediumZombieLevel; }
        private set
        {
            if (_mediumZombieLevel != value)
            {
                if (value <= 0)
                {
                    _mediumZombieLevel = 1;
                }
                else
                {
                    _mediumZombieLevel = value;
                }
                SaveManager.Instance.InitiateSave();
            }
        }
    }

    public int LargeZombieLevel
    {
        get { return _largeZombieLevel; }
        private set
        {
            if (_largeZombieLevel != value)
            {
                if (value <= 0)
                {
                    _largeZombieLevel = 1;
                }
                else
                {
                    _largeZombieLevel = value;
                }
                SaveManager.Instance.InitiateSave();
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
    }

    public ZombieLevel GetZombieData(ZombieType type)
    {
        return type switch
        {
            ZombieType.Small => _smallZombieData.GetLevel(SmallZombieLevel),
            ZombieType.Medium => _mediumZombieData.GetLevel(MediumZombieLevel),
            ZombieType.Large => _largeZombieData.GetLevel(LargeZombieLevel),
            ZombieType.Playground => _smallZombieData.GetLevel(SmallZombieLevel), // Adjust as needed.
            _ => null,
        };
    }

    public bool IsZombieMaxLevel(ZombieType type)
    {
        return type switch
        {
            ZombieType.Small => SmallZombieLevel >= _smallZombieData.MaxLevel,
            ZombieType.Medium => MediumZombieLevel >= _mediumZombieData.MaxLevel,
            ZombieType.Large => LargeZombieLevel >= _largeZombieData.MaxLevel,
            ZombieType.Playground => SmallZombieLevel >= _smallZombieData.MaxLevel, // Adjust as needed.
            _ => false,
        };
    }

    public ZombieLevel GetZombieDataNextLevel(ZombieType type)
    {
        return type switch
        {
            ZombieType.Small => _smallZombieData.GetLevel(SmallZombieLevel + 1),
            ZombieType.Medium => _mediumZombieData.GetLevel(MediumZombieLevel + 1),
            ZombieType.Large => _largeZombieData.GetLevel(LargeZombieLevel + 1),
            ZombieType.Playground => _smallZombieData.GetLevel(SmallZombieLevel + 1), // Adjust as needed.
            _ => new ZombieLevel(), // Adjust as needed.
        };
    }

    public Zombie GetZombiePrefab(ZombieType type)
    {
        return type switch
        {
            ZombieType.Small => _smallZombiePrefab,
            ZombieType.Medium => _mediumZombiePrefab,
            ZombieType.Large => _largeZombiePrefab,
            ZombieType.Playground => _playgroundZombiePrefab,
            _ => null,
        };
    }

    public Defender GetDefenderPrefab(DefenderType type, DefenderRangedType rangedType = default)
    {
        return type switch
        {
            DefenderType.Melee => _meleeDefenderPrefab,
            DefenderType.Ranged => rangedType switch
            {
                DefenderRangedType.Archer => _archerDefenderPrefab,
                _ => null,
            },
            _ => null,
        };
    }

    public Civilian GetRandomCivilian() => _civilianPrefabs.Count > 0 ? _civilianPrefabs[Random.Range(0, _civilianPrefabs.Count)] : null;

    public Sprite GetZombieSprite(ZombieType type)
    {
        return type switch
        {
            ZombieType.Small => _smallZombieSprite,
            ZombieType.Medium => _mediumZombieSprite,
            ZombieType.Large => _largeZombieSprite,
            ZombieType.Playground => _smallZombieSprite, // Adjust as needed.
            _ => null,
        };
    }

    public void LoadData(ISaveableObject item)
    {
        if (item is CharacterData data)
        {
            SmallZombieLevel = data.SmallZombieLevel;
            MediumZombieLevel = data.MediumZombieLevel;
            LargeZombieLevel = data.LargeZombieLevel;
        }
        else
        {
            SmallZombieLevel = 1;
            MediumZombieLevel = 1;
            LargeZombieLevel = 1;
        }
    }

    public ISaveableObject GetData() =>
        new CharacterData
        {
            SmallZombieLevel = SmallZombieLevel,
            MediumZombieLevel = MediumZombieLevel,
            LargeZombieLevel = LargeZombieLevel
        };

    public void UpgradeZombie(ZombieType type)
    {
        switch (type)
        {
            case ZombieType.Small:
                SmallZombieLevel++;
                break;
            case ZombieType.Medium:
                MediumZombieLevel++;
                break;
            case ZombieType.Large:
                LargeZombieLevel++;
                break;
        }
    }

    public Civilian GetRandomCivlian() => _civilianPrefabs[Random.Range(0, _civilianPrefabs.Count)];
}
