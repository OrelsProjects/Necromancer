using UnityEngine;

public enum ZombieType
{
    ZombieLab,
}

public struct CharacterData : ISaveableObject
{
    public int BasicZombieLevel;
}

public class CharactersManager : MonoBehaviour, ISaveable
{
    public static CharactersManager Instance;

    private InventoryManager _inventory;

    [SerializeField]
    private GameObject _basicZombiePrefab;
    [SerializeField]
    private Sprite _basicZombieSprite;
    [SerializeField]
    private ZombieData _basicZombieData;

    private int _basicZombieLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
    }

    private void Start()
    {
        _inventory = InventoryManager.Instance;
    }

    private void InitZombies()
    {
        _basicZombieData.SetCurrentLevel(_basicZombieLevel);
    }

    public ZombieLevel GetZombieData(ZombieType type)
    {
        return type switch
        {
            ZombieType.ZombieLab => _basicZombieData.GetCurrentLevel(),
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

    public void LoadData()
    {
        _basicZombieLevel = SaveManager.Instance.GetData<CharacterData>().BasicZombieLevel;
        _basicZombieLevel = 4;
        InitZombies();
    }

    public ISaveableObject GetData()
    {
        return new CharacterData
        {
            BasicZombieLevel = _basicZombieLevel,
        };
    }
}