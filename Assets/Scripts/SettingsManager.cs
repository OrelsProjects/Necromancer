using UnityEngine;

public struct SettingBool
{
    public bool value;
    public bool defaultValue;
    public string name;
}

public struct SettingsData : ISaveableObject
{
    public SettingBool Sound;
    public SettingBool Vibration;

    public string GetName() => GetType().FullName;
    public readonly string GetObjectType() =>
        GetType().FullName;

}

public static class Settings
{
    public static SettingBool Vibration = new()
    {
        value = false,
        defaultValue = false,
        name = "Vibration"
    };
    public static SettingBool Sound = new()
    {
        value = false,
        defaultValue = false,
        name = "Sound"
    };
}

public class SettingsManager : DisableMapMovement, ISaveable
{
    public static SettingsManager Instance;

    #region UI Fields
    [SerializeField]
    private GameObject _settingsUI;

    [Header("UI")]
    [Header("Sound UI")]
    [SerializeField]
    private GameObject _enabledSound;
    [SerializeField]
    private GameObject _disabledSound;
    [Header("Vibration UI")]
    [HideInInspector]
    [SerializeField]
    private GameObject _enabledVibration;
    [HideInInspector]
    [SerializeField]
    private GameObject _disabledVibration;

    #endregion

    [HideInInspector]
    public bool Vibration
    {
        get => Settings.Vibration.value;
        set
        {
            Settings.Vibration.value = value;
            UpdateVibrationUI();
            SaveManager.Instance.SaveItem(GetData());
        }
    }

    [HideInInspector]
    public bool Sound
    {
        get => Settings.Sound.value;
        set
        {
            Settings.Sound.value = value;
            if (Settings.Sound.value)
            {
                AudioListener.volume = 1f;
            }
            else
            {
                AudioListener.volume = 0f;
            }
            UpdateSoundUI();
            SaveManager.Instance.SaveItem(GetData());
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

    private void Start()
    {
        Application.targetFrameRate = 144;
        UpdateSoundUI();
        //UpdateVibrationUI();
    }

    private void setDefaults()
    {
        Sound = Settings.Sound.defaultValue;
        // Vibration = Settings.Vibration.defaultValue;
    }

    #region Modifiers
    public void ToggleSound() => Sound = !Sound;

    public void ToggleVibration() => Vibration = !Vibration;

    #endregion

    #region UI
    private void UpdateSoundUI()
    {
        if (enabled)
        {
            _enabledSound.SetActive(Sound);
            _disabledSound.SetActive(!Sound);
        }
    }

    private void UpdateVibrationUI()
    {
        if (enabled)
        {
            _enabledVibration.SetActive(Vibration);
            _disabledVibration.SetActive(!Vibration);
        }
    }

    public void ShowSettings()
    {
        _settingsUI.SetActive(true);

    }

    public void HideSettings()
    {
        _settingsUI.SetActive(false);
    }

    public void CloseGame() =>
        SaveManager.Instance.InitiateSave(closeAppAfterSave: true);


    #endregion

    public ISaveableObject GetData() => new SettingsData()
    {
        Sound = Settings.Sound,
        Vibration = Settings.Vibration
    };

    public void LoadData(ISaveableObject item)
    {
        if (item is SettingsData data)
        {
            Sound = data.Sound.value;
            //Vibration = data.Vibration;
        }
        else
        {
            Debug.LogError("Loaded data is not of type SettingsData");
        }
    }

    public string GetObjectName() => new SettingsData().GetName();
}
