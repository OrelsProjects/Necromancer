using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public int Currency { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Currency = 10;
            DontDestroyOnLoad(gameObject);
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
}