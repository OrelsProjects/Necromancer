using UnityEngine;

public struct SettingsData : ISaveableObject
{
    public bool Sound;
    public bool Vibration;

    public string GetObjectType()
    {
        return GetType().FullName;
    }
}

public static class Settings
{
    public static bool Vibration = true;
    public static bool Sound = true;
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
        get => Settings.Vibration;
        set
        {
            Settings.Vibration = value;
            UpdateVibrationUI();
            SaveManager.Instance.SaveItem(GetData());
        }
    }

    [HideInInspector]
    public bool Sound
    {
        get => Settings.Sound;
        set
        {
            Settings.Sound = value;
            if (Settings.Sound)
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
            Settings.Sound = true;
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

    #region Modifiers
    public void ToggleSound() => Sound = !Sound;


    public void ToggleVibration() => Vibration = !Vibration;

    #endregion

    #region UI
    private void UpdateSoundUI()
    {
        _enabledSound.SetActive(Sound);
        _disabledSound.SetActive(!Sound);
    }

    private void UpdateVibrationUI()
    {
        _enabledVibration.SetActive(Vibration);
        _disabledVibration.SetActive(!Vibration);
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
            Sound = data.Sound;
            //Vibration = data.Vibration;
        }
        else
        {
            Debug.LogError("Loaded data is not of type SettingsData");
        }
    }
}
