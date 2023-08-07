using UnityEngine;

public enum ZombieType
{
    ZombieLab,
}

public struct CharacterData : ISaveableObject
{
    public int BasicZombieLevel;

    public string GetObjectType()
    {
        return GetType().FullName;
    }
}

public class CharactersManager : MonoBehaviour, ISaveable
{
    public static CharactersManager Instance;


    [SerializeField]
    private GameObject _basicZombiePrefab;
    [SerializeField]
    private Sprite _basicZombieSprite;
    [SerializeField]
    private ZombieData _basicZombieData; // TODO: Maybe change it and make the data inside immutable (The _currentLevel)

    private int _basicZombieLevel = 1;

    public int BasicZombieLevel
    {
        get { return _basicZombieLevel; }
        private set
        {
            if (_basicZombieLevel != value)
            {
                if (value <= 0)
                {
                    _basicZombieLevel = 1;
                }
                else
                {
                    _basicZombieLevel = value;
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
            ZombieType.ZombieLab => _basicZombieData.GetLevel(BasicZombieLevel),
            _ => new(),
        };
    }

    public ZombieLevel GetZombieDataNextLevel(ZombieType type)
    {
        return type switch
        {
            ZombieType.ZombieLab => _basicZombieData.GetLevel(BasicZombieLevel + 1),
            _ => new(),
        };
    }

    public GameObject GetZombiePrefab(ZombieType type)
    {
        return type switch
        {
            ZombieType.ZombieLab => _basicZombiePrefab,
            _ => null,
        };
    }

    public Sprite GetZombieSprite(ZombieType type)
    {
        return type switch
        {
            ZombieType.ZombieLab => _basicZombieSprite,
            _ => null,
        };
    }

    public void LoadData(ISaveableObject item)
    {
        if (item is CharacterData data)
        {
            BasicZombieLevel = data.BasicZombieLevel;
        }
        else
        {
            BasicZombieLevel = 1;
        }
    }

    public ISaveableObject GetData()
    {
        return new CharacterData
        {
            BasicZombieLevel = BasicZombieLevel,
        };
    }

    public void UpgradeZombie(ZombieType type)
    {
        switch (type)
        {
            case ZombieType.ZombieLab:
                BasicZombieLevel++;
                break;
        }
    }
}