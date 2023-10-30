using System.Collections.Generic;
using UnityEngine;


public enum SourceType
{
    Background,
    UI,
    SFX,
}

public enum BackgroundSoundTypes
{
    Map,
    ZombiesSpawned,
}


public enum UISoundTypes
{
    ButtonClick,
    Purchase,
    CoinsCollect,
    RoundWin,
    RoundLose,
}

[System.Serializable]
public struct AreaClip
{
    public Areas area;
    public AudioClip clip;
}

public class SoundsManager : MonoBehaviour
{

    public static SoundsManager Instance;

    [Header("Audio Clips")]
    [Header("Background Clips")]
    [SerializeField]
    private AudioClip _mapSound;
    [SerializeField]
    private AudioClip _zombiesSpawned;

    [Header("UI Clips")]
    [SerializeField]
    private AudioClip _buttonClickSound;
    [SerializeField]
    private AudioClip _purchaseSound;
    [SerializeField]
    private AudioClip _coinsCollectSonud;
    [SerializeField]
    private AudioClip _roundLose;
    [SerializeField]
    private AudioClip _roundWin;

    [Header("Areas Clips")]
    [SerializeField]
    private List<AreaClip> _areasClips;


    [Header("Audio Listeners")]
    [SerializeField]
    private AudioSource _backgroundAudioSource;

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

    public void StopAll()
    {
        if (_backgroundAudioSource != null)
        {
            _backgroundAudioSource.Stop();
        }
    }

    public static AudioClip GetAreaSound(Areas area)
    {
        AreaClip clip = Instance._areasClips.Find(a => a.area == area);
        if (clip.clip != null)
        {
            return clip.clip;
        }
        return Instance._mapSound;
    }

    public static AudioClip GetBackgroundSound(BackgroundSoundTypes type) => type switch
    {
        BackgroundSoundTypes.Map => Instance._mapSound,
        BackgroundSoundTypes.ZombiesSpawned => Instance._zombiesSpawned,
        _ => throw new System.NotImplementedException(),
    };

    public static AudioClip GetUISound(UISoundTypes type) => type switch
    {
        UISoundTypes.ButtonClick => Instance._buttonClickSound,
        UISoundTypes.Purchase => Instance._purchaseSound,
        UISoundTypes.CoinsCollect => Instance._coinsCollectSonud,
        UISoundTypes.RoundWin => Instance._roundWin,
        UISoundTypes.RoundLose => Instance._roundLose,
        _ => throw new System.NotImplementedException(),
    };
}
