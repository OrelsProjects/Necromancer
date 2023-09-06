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

    public delegate void CurrencyChangedDelegate(int newCurrency);

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
        else
        {
            Destroy(this);
        }
    }

    public bool CanAfford(int cost) => Currency >= cost;


    public void AddCurrency(int amount) => Currency += amount;

    public bool UseCurrency(int amount)
    {
        if (CanAfford(amount))
        {
            Currency -= amount;
            AudioSourceHelper.PlayClipAtPoint(UISoundTypes.Purchase);
            return true;
        }
        return false;
    }

    public ISaveableObject GetData()
    {
        return new InventoryData
        {
            Currency = _currency
        };
    }

    public void LoadData(ISaveableObject item)
    {
        if (item is InventoryData data)
        {
            if (data.Currency == 0)
            {
                Currency = 50;
            }
            else
            {
                Currency = data.Currency;
            }
        }
    }
}
