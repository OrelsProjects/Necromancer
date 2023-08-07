using System.Collections;
using UnityEngine;

public struct InventoryData : ISaveableObject
{
    public int Currency;

    public string GetObjectType()
    {
        return GetType().FullName;
    }
}

public class InventoryManager : MonoBehaviour, ISaveable
{
    public static InventoryManager Instance { get; private set; }

    // Define a delegate for the currency change event
    public delegate void CurrencyChangedDelegate(int newCurrency);

    // Declare the currency change event using the delegate
    public event CurrencyChangedDelegate OnCurrencyChanged;

    private int _currency;

    public int Currency
    {
        get { return _currency; }
        private set
        {
            if (_currency != value)
            {
                _currency = value;
                Debug.Log("Currency updated, new value: " + _currency.ToString());
                OnCurrencyChanged?.Invoke(value);
                SaveManager.Instance.InitiateSave();
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        if (CanAfford(amount))
        {
            Currency -= amount;
            return true;
        }
        return false;
    }

    public ISaveableObject GetData()
    {
        Debug.Log("Getting inventory data, currency: " + _currency.ToString());
        return new InventoryData
        {
            Currency = _currency
        };
    }

    public void LoadData(ISaveableObject item)
    {
        if (item is InventoryData data)
        {
            Currency = data.Currency;
        }
    }
}
