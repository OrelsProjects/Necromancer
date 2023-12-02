using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// To add another area, add another enum and go to MapController to add another Map Loader function.
// Then in your area object set the raid function to call the new Map Loader function.
public enum Areas
{
    Area1,
    Area2,
    Area3,
    Area4,
    Area5,
    Playground,
}

public enum AreaState
{
    Default,
    Zombified,
}

public struct AreasData : ISaveableObject
{

    public Dictionary<Areas, AreaState> AreasState;
    public Dictionary<Areas, int> AreasLevels;
    public Dictionary<Areas, DateTime> AreasProductionTime;
    public bool IsCompletedScreenShown;

    public readonly string GetObjectType() => GetType().FullName;
    public readonly string GetName() => GetObjectType();
}

public class AreasManager : MonoBehaviour, ISaveable
{
    public static AreasManager Instance;

    [SerializeField]
    private GameObject Lab1;
    [SerializeField]
    private GameObject Lab2;
    [SerializeField]
    private GameObject Lab3;

    [SerializeField]
    private int _maxMinutesToCollectProduction = 60;

    [SerializeField]
    private List<AreaData> _areasData = new();

    public delegate void AreasLevelChangeDelegate();
    private event AreasLevelChangeDelegate OnAreaLevelChanged;
    private bool _isCompletedScreenShown = false;
    private Dictionary<Areas, int> _areasLevels = new();
    public Dictionary<Areas, int> AreasLevels
    {
        get { return _areasLevels; }
    }

    public delegate void AreasStateChangeDelegate();
    private event AreasStateChangeDelegate OnAreaStateChanged;
    private Dictionary<Areas, AreaState> _areasState = new();
    public Dictionary<Areas, AreaState> AreasState
    {
        get { return _areasState; }
    }

    /// <summary>
    /// When did the area's last production start time was.
    /// </summary>
    private Dictionary<Areas, DateTime> _areaLastProductionCollectionTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    private void NotifyChangesAreaState()
    {
        OnAreaStateChanged?.Invoke();
    }

    private void NotifyChangesAreaLevel()
    {
        OnAreaLevelChanged?.Invoke();
    }

    public void SubscribeToAreasStateChange(AreasStateChangeDelegate delegateSubscribe)
    {
        OnAreaStateChanged += delegateSubscribe;
    }

    public void SubscribeToAreasLevelChange(AreasLevelChangeDelegate delegateSubscribe)
    {
        OnAreaLevelChanged += delegateSubscribe;
    }

    public void UnsubscribeFromAreasStateChange(AreasStateChangeDelegate delegateUnsubscribe)
    {
        OnAreaStateChanged -= delegateUnsubscribe;
    }

    public void UnsubscribeFromAreasLevelChange(AreasLevelChangeDelegate delegateUnsubscribe)
    {
        OnAreaLevelChanged -= delegateUnsubscribe;
    }

    public bool IsGameCompleted() => _areasState.All(a => a.Value == AreaState.Zombified);

    public void CompleteGame()
    {
        if (!_isCompletedScreenShown && IsGameCompleted())
        {
            Game.Instance.SetState(GameState.Completed);
            UIController.Instance.ShowCompletedScreen();
            _isCompletedScreenShown = true;
        }
    }

    public void CollectProduction()
    {
        int production = CalculateProduction();
        if (production == 0)
        { // Don't reset timer if nothing was produced yet.
            return;
        }

        Dictionary<Areas, DateTime> newAreaLastProductionConsumeTime
        = _areaLastProductionCollectionTime.ToDictionary(entry => entry.Key, entry => entry.Value);

        foreach (var area in _areaLastProductionCollectionTime)
        {
            if (IsAreaZombified(area.Key))
            {
                newAreaLastProductionConsumeTime[area.Key] = DateTime.Now;
            }
        }

        _areaLastProductionCollectionTime = newAreaLastProductionConsumeTime;
        SaveData();
    }

    public void UpgradeArea(Areas area)
    {
        _areasLevels[area]++;
        NotifyChangesAreaLevel();
        SaveData();
    }

    public GameObject GetLab(Areas area) =>
        GetAreaLevel(area) switch
        {
            1 => Lab1,
            2 => Lab2,
            3 => Lab3,
            _ => null,
        };

    public GameObject GetNextLab(Areas area) =>
        GetAreaLevel(area) switch
        {
            1 => Lab2,
            2 or 3 => Lab3,
            _ => null,
        };


    public AreaData GetAreaData(Areas area) => _areasData.FirstOrDefault(a => a.Area == area);

    public int GetAreaLevel(Areas area)
    {
        if (_areasLevels.ContainsKey(area))
        {
            return _areasLevels[area];
        }
        return 0;
    }

    public int GetMaxLevel(Areas area)
    {
        if (_areasLevels.ContainsKey(area))
        {
            return GetAreaData(area).MaxLevel;
        }
        return 0;
    }

    public bool IsAreaZombified(Areas area)
    {
        if (_areasState.ContainsKey(area))
        {
            return _areasState[area] == AreaState.Zombified;
        }
        return false;
    }

    public void ZombifyArea(Areas area)
    {
        if (!_areasState.ContainsKey(area))
        {
            _areasState.TryAdd(area, AreaState.Zombified);
        }
        else
        {
            _areasState[area] = AreaState.Zombified;
        }
        _areaLastProductionCollectionTime[area] = DateTime.Now;

        NotifyChangesAreaState();
        SaveData();
    }

    public bool IsAreaMaxLevel(Areas area)
    {
        if (_areasLevels.ContainsKey(area))
        {
            AreaData areaData = GetAreaData(area);
            int areaLevel = _areasLevels[area];
            if (areaData == null)
            {
                return false;
            }
            return areaLevel >= GetAreaData(area).MaxLevel;
        }
        return false;
    }

    public void SaveData() => SaveManager.Instance.SaveItem(GetData());

    public ISaveableObject GetData()
    {
        return new AreasData
        {
            AreasState = _areasState,
            AreasLevels = _areasLevels,
            AreasProductionTime = _areaLastProductionCollectionTime,
            IsCompletedScreenShown = _isCompletedScreenShown,
        };
    }

    public void LoadData(ISaveableObject item)
    {
        if (item is AreasData data)
        {
            _areasState = data.AreasState; // Trigger event after loading data   
            _areasLevels = data.AreasLevels;
            _areaLastProductionCollectionTime = data.AreasProductionTime;
            _isCompletedScreenShown = data.IsCompletedScreenShown;
        }
        _areasLevels ??= new();
        _areasState ??= new();
        _areaLastProductionCollectionTime ??= new();

        // If some areas were not added, we set their default values
        _areasData.ForEach(a =>
        {
            if (!_areasLevels.ContainsKey(a.Area))
            {
                _areasLevels.TryAdd(a.Area, 1);
            }
            if (!_areasState.ContainsKey(a.Area))
            {
                _areasState.TryAdd(a.Area, AreaState.Default);
            }
            if (!_areaLastProductionCollectionTime.ContainsKey(a.Area))
            {
                _areaLastProductionCollectionTime.TryAdd(a.Area, DateTime.Now);
            }
        });
        NotifyChangesAreaLevel();
        NotifyChangesAreaState();
    }

    public int CalculateProduction()
    {
        int totalProduction = 0;
        if (_areaLastProductionCollectionTime != null)
        {
            foreach (var area in _areaLastProductionCollectionTime)
            {
                if (IsAreaZombified(area.Key))
                {
                    TimeSpan timeSpan = DateTime.Now - area.Value;
                    int minutes = Mathf.Clamp((int)timeSpan.TotalMinutes, 0, _maxMinutesToCollectProduction);
                    int areaLevel = GetAreaLevel(area.Key);
                    int currencyPerMinute = GetAreaData(area.Key).GetAreaLevel(areaLevel).CurrencyPerMinute;
                    int production = minutes * currencyPerMinute;
                    totalProduction += production;
                }
            }
        }
        return totalProduction;
    }

    public int CalculateMaxProduction()
    {
        int totalProduction = 0;
        if (_areaLastProductionCollectionTime != null)
        {
            foreach (var area in _areaLastProductionCollectionTime)
            {
                if (IsAreaZombified(area.Key))
                {
                    int areaMaxLevel = GetMaxLevel(area.Key);
                    int currencyPerMinute = GetAreaData(area.Key).GetAreaLevel(areaMaxLevel).CurrencyPerMinute;
                    int production = _maxMinutesToCollectProduction * currencyPerMinute;
                    totalProduction += production;
                }
            }
        }
        return totalProduction;

    }

    public string GetObjectName() => new AreasData().GetName();
}
