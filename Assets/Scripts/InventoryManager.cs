using System.Collections.Generic;
using UnityEngine;

public struct InventoryData : ISaveableObject
{
    public int Currency;
    public List<ZombieType> AcquiredZombies;

    public readonly string GetName() => GetObjectType();
    public readonly string GetObjectType() => GetType().FullName;

}

public class InventoryManager : MonoBehaviour, ISaveable
{
    public static InventoryManager Instance { get; private set; }

    public delegate void CurrencyChangedDelegate(int newCurrency, int oldCurrency);
    public event CurrencyChangedDelegate OnCurrencyChanged;

    private List<ZombieType> _acquiredZombies = new();

    public List<ZombieType> AcquiredZombies
    {
        get => _acquiredZombies;
        private set
        {
            _acquiredZombies = value;
        }
    }

    public List<ZombieType> AcquireableZombies
    {
        get => new(new[] { ZombieType.Small, ZombieType.Medium, ZombieType.Large });
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

    public bool CanAfford(int cost) => cost <= 0 || Currency >= cost;

    public void AddCurrency(int amount)
    {
        Currency += amount;
        SaveManager.Instance.SaveItem(GetData());
    }

    public bool UseCurrency(int amount)
    {
        Debug.Log("About to use currency: " + amount + " Can afford? " + CanAfford(amount));
        if (amount < 0)
        {
            Debug.LogError("Trying to use negative currency: " + amount);
            return false;
        }
        if (CanAfford(amount))
        {
            Currency -= amount;
            AudioSourceHelper.PlayClipAtPoint(UISoundTypes.Purchase);
            SaveManager.Instance.SaveItem(GetData());
            return true;
        }
        return false;
    }

    public bool IsZombieAcquired(ZombieType zombie) => _acquiredZombies.Contains(zombie);

    public void AcquireZombie(ZombieType zombie)
    {
        if (_acquiredZombies.Contains(zombie))
        {
            return;
        }
        _acquiredZombies.Add(zombie);
        SaveManager.Instance.SaveItem(GetData());
    }

    public ISaveableObject GetData()
    {
        return new InventoryData
        {
            Currency = _currency,
            AcquiredZombies = _acquiredZombies
        };
    }

    public void LoadData(ISaveableObject item)
    {
        if (item is InventoryData data)
        {
            Currency = data.Currency;
            if (data.AcquiredZombies == null || data.AcquiredZombies.Count == 0)
            {
                _acquiredZombies = new List<ZombieType>();
                SaveManager.Instance.SaveItem(GetData());
            }
            else
            {
                _acquiredZombies = new List<ZombieType>(data.AcquiredZombies);
            }
        }
    }

    public string GetObjectName() => new InventoryData().GetName();
}
