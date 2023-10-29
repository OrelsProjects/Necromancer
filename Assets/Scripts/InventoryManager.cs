using System.Collections.Generic;
using UnityEngine;

public struct InventoryData : ISaveableObject
{
    public int Currency;
    public ZombieType[] AcquiredZombies;
    
    public string GetObjectType()
    {
        return GetType().FullName;
    }
}

public class InventoryManager : MonoBehaviour, ISaveable
{
    public static InventoryManager Instance { get; private set; }

    public delegate void CurrencyChangedDelegate(int newCurrency, int oldCurrency);
    public event CurrencyChangedDelegate OnCurrencyChanged;

    private List<ZombieType> _acquiredZombies;

    public List<ZombieType> AcquiredZombies
    {
        get => _acquiredZombies;
        private set
        {
            _acquiredZombies = value;
                    }
    }

    private int _currency;

    public int Currency
    {
        get => _currency;
        private set
        {
            int oldValue = _currency;
            _currency = value;
            OnCurrencyChanged?.Invoke(value, oldValue);
            SaveManager.Instance.InitiateSave();
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

    public void AcquireZombie(ZombieType zombie)
    {
        if (_acquiredZombies.Contains(zombie))
        {
            return;
        }
        _acquiredZombies.Add(zombie);
    }

    public ISaveableObject GetData()
    {
        return new InventoryData
        {
            Currency = _currency,
            AcquiredZombies = _acquiredZombies?.ToArray() ?? new ZombieType[0]
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
            if (data.AcquiredZombies == null || data.AcquiredZombies.Length == 0)
            {
                _acquiredZombies = new List<ZombieType>();
                _acquiredZombies.AddRange(new[] { ZombieType.Small });
                SaveManager.Instance.InitiateSave();
            }
            else
            {
                _acquiredZombies = new List<ZombieType>(data.AcquiredZombies);
            }
        }
    }
}
