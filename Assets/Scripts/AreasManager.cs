using System;
using System.Collections.Generic;
using UnityEngine;

public enum Areas
{
    Area1,
}

public enum AreaState
{
    Default,
    Zombified,
}

public struct AreasData : ISaveableObject
{
    public Dictionary<Areas, AreaState> AreasState;

    public string GetObjectType()
    {
        return GetType().FullName;
    }
}

public class AreasManager : MonoBehaviour, ISaveable
{
    public static AreasManager Instance;

    public delegate void AreasStateChangeDelegate(Dictionary<Areas, AreaState> _areasState);
    public event AreasStateChangeDelegate OnAreaStateChanged;

    private Dictionary<Areas, AreaState> _areasState = new();
    public Dictionary<Areas, AreaState> AreasState
    {
        get { return _areasState; }
        set
        {
            _areasState = value;
            OnAreaStateChanged?.Invoke(_areasState);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void AreaZombified(Areas area)
    {
        if (!_areasState.ContainsKey(area))
        {
            _areasState.Add(area, AreaState.Zombified);
        }
        else
        {
            _areasState[area] = AreaState.Zombified;
        }

        // Set the property to trigger the event
        AreasState = _areasState;

        SaveManager.Instance.SaveItem(GetData());
    }

    public ISaveableObject GetData()
    {
        return new AreasData
        {
            AreasState = _areasState
        };
    }

    public void LoadData(ISaveableObject item)
    {
        if (item is AreasData data)
        {
            _areasState = data.AreasState;
            AreasState = _areasState; // Trigger event after loading data
        }
    }
}
