using UnityEngine;

public class CharactersManager : MonoBehaviour, ISaveable
{
    public static CharactersManager Instance;

    private InventoryManager _inventory;

    private ZombieData _labZombieData;

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
    public IDTO GetData()
    {
        return _labZombieData.GetCurrentLevel();
    }

    public void LoadData()
    {
        ZombieLevelDTO currentZombieLevel = SaveManager.Instance.GetData<ZombieLevelDTO>();
        if (currentZombieLevel != null)
        {
            _labZombieData = new(currentZombieLevel.Level);
        }
        else
        {
            _labZombieData = new(1);
        }
    }
}