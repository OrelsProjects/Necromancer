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
            Currency = 0;
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

    public IDTO GetData()
    {
        return new InventoryDTO
        {
            Currency = 2932
        };
    }

    public void LoadData()
    {
        InventoryDTO inventoryDTO = SaveManager.Instance.GetData<InventoryDTO>();
        if (inventoryDTO != null)
        {
            Currency = inventoryDTO.Currency;
        }
    }
}