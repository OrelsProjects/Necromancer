using UnityEngine;

public struct SettingsData : ISaveableObject
{
    public float Volume;

    public string GetObjectType()
    {
        return GetType().FullName;
    }
}

public class Settings : MonoBehaviour, ISaveable
{
    private float _volume = 1f;

    public float Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            SaveManager.Instance.SaveItem(GetData());
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 144;
    }

    private void Update()
    {
        AudioListener.volume = _volume;
    }

    public ISaveableObject GetData() => new SettingsData() { Volume = _volume };

    public void LoadData(ISaveableObject item)
    {
        if (item is SettingsData data)
        {
            _volume = data.Volume;
        }
        else
        {
            Debug.LogError("Loaded data is not of type SettingsData");
        }
    }
}
