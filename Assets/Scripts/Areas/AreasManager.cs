using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// To add another area, add another enum and go to MapController to add another Map Loader function.
// Then in your area object set the raid function to call the new Map Loader function.
public enum Areas {
    Area1,
    Area2,
    Area3,
    Area4,
    Area5,
    Playground,
}

public enum AreaState {
    Default,
    Zombified,
}

public struct AreasData : ISaveableObject {

    public Dictionary<Areas, AreaState> AreasState;
    public Dictionary<Areas, int> AreasLevels;

    public string GetObjectType() {
        return GetType().FullName;
    }
}

public class AreasManager : MonoBehaviour, ISaveable {
    public static AreasManager Instance;

    [SerializeField]
    private GameObject Lab1;
    [SerializeField]
    private GameObject Lab2;
    [SerializeField]
    private GameObject Lab3;

    [SerializeField]
    private List<AreaData> _areasData = new();

    public delegate void AreasLevelChangeDelegate();
    private event AreasLevelChangeDelegate OnAreaLevelChanged;
    private Dictionary<Areas, int> _areasLevels = new();
    public Dictionary<Areas, int> AreasLevels {
        get { return _areasLevels; }
    }

    public delegate void AreasStateChangeDelegate();
    private event AreasStateChangeDelegate OnAreaStateChanged;
    private Dictionary<Areas, AreaState> _areasState = new();
    public Dictionary<Areas, AreaState> AreasState {
        get { return _areasState; }
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void NotifyChangesAreaState() {
        OnAreaStateChanged?.Invoke();
    }

    private void NotifyChangesAreaLevel() {
        OnAreaLevelChanged?.Invoke();
    }

    public void SubscribeToAreasStateChange(AreasStateChangeDelegate delegateSubscribe) {
        OnAreaStateChanged += delegateSubscribe;
    }

    public void SubscribeToAreasLevelChange(AreasLevelChangeDelegate delegateSubscribe) {
        OnAreaLevelChanged += delegateSubscribe;
    }

    public void UnsubscribeFromAreasStateChange(AreasStateChangeDelegate delegateUnsubscribe) {
        OnAreaStateChanged -= delegateUnsubscribe;
    }

    public void UnsubscribeFromAreasLevelChange(AreasLevelChangeDelegate delegateUnsubscribe) {
        OnAreaLevelChanged -= delegateUnsubscribe;
    }

    public void SelectAreaForUpgrade(Areas area) {
        UIController.Instance.ShowAreaUpgrade(area);
    }

    public void CloseAreaUpgrade() {
        UIController.Instance.HideAreaUpgrade();
    }

    public void UpgradeArea(Areas area) {
        _areasLevels[area]++;
        NotifyChangesAreaLevel();
        SaveData();
    }

    public GameObject GetLab(Areas area) =>
        GetAreaLevel(area) switch {
            1 => Lab1,
            2 => Lab2,
            3 => Lab3,
            _ => null,
        };

    public GameObject GetNextLab(Areas area) =>
        GetAreaLevel(area) switch {
            1 => Lab2,
            2 or 3 => Lab3,
            _ => null,
        };


    public AreaData GetAreaData(Areas area) {
        return _areasData.FirstOrDefault(a => a.Area == area);
    }

    public int GetAreaLevel(Areas area) {
        if (_areasLevels.ContainsKey(area)) {
            return _areasLevels[area];
        }
        return 0;
    }

    public void AreaZombified(Areas area) {
        if (!_areasState.ContainsKey(area)) {
            _areasState.Add(area, AreaState.Zombified);
        } else {
            _areasState[area] = AreaState.Zombified;
        }

        NotifyChangesAreaState();
        SaveData();
    }

    public bool IsAreaMaxLevel(Areas area) {
        if (_areasLevels.ContainsKey(area)) {
            AreaData areaData = GetAreaData(area);
            int areaLevel = _areasLevels[area];
            if (areaData == null) {
                return false;
            }
            return areaLevel >= GetAreaData(area).MaxLevel;
        }
        return false;
    }

    public void SaveData() {
        SaveManager.Instance.SaveItem(GetData());
    }

    public ISaveableObject GetData() {
        return new AreasData {
            AreasState = _areasState,
            AreasLevels = _areasLevels,
        };
    }

    public void LoadData(ISaveableObject item) {
        if (item is AreasData data) {
            _areasState = data.AreasState; // Trigger event after loading data   
            _areasLevels = data.AreasLevels;
        }
        _areasLevels ??= new();
        _areasState ??= new();

        _areasData.ForEach(a => {
            if (!_areasLevels.ContainsKey(a.Area)) {
                _areasLevels.Add(a.Area, 1);
            }
            if (!_areasState.ContainsKey(a.Area)) {
                _areasState.Add(a.Area, AreaState.Default);
            }
        });
        NotifyChangesAreaLevel();
        NotifyChangesAreaState();
    }
}
