using System.Collections.Generic;
using UnityEngine;

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

    public Dictionary<string, string> GetData()
    {
        return new Dictionary<string, string>()
        {
            { "Currency", Currency.ToString() }
        };
    }

    public void LoadData()
    {
        Dictionary<string, string> newCurrency = SaveManager.Instance.GetData(new List<string>() { "Currency" });
        // if newCurrency has "Currency", set Currency to newCurrency["Currency"] else set Currency to 0
        Currency = newCurrency.ContainsKey("Currency") ? int.Parse(newCurrency["Currency"]) : 0;
    }
}