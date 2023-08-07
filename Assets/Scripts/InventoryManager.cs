using UnityEngine;

public struct InventoryData : ISaveableObject
{
    public int Currency;
}

public class InventoryManager : MonoBehaviour, ISaveable
{
    public static InventoryManager Instance { get; private set; }

    public int Currency { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Currency = 10;
            DontDestroyOnLoad(this);
        }
    }

    public bool CanAfford(int cost)
    {
        return Currency >= cost;
    }

    public void AddCurrency(int amount)
    {
        Currency += amount;
    }

    public bool UseCurrency(int amount)
    {
        if (Currency < amount)
        {
            return false;
        }
        Currency -= amount;
        return true;
    }

    public ISaveableObject GetData()
    {
        return new InventoryData
        {
            Currency = Currency
        };
    }

    public void LoadData()
    {
        Currency = SaveManager.Instance.GetData<InventoryData>().Currency;
    }
}