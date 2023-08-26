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
    Area,
    ZombiesSpawned,
}


public enum UISoundTypes
{
    ButtonClick,
    Purchase,
    CoinsCollect
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
    private AudioClip _map;
    [SerializeField]
    private AudioClip _zombiesSpawned;

    [Header("UI Clips")]
    [SerializeField]
    private AudioClip _buttonClickSound;
    [SerializeField]
    private AudioClip _purchaseSound;
    [SerializeField]
    private AudioClip _coinsCollectSonud;

    [Header("Areas Clips")]
    [SerializeField]
    private List<AreaClip> _areasClips;


    [Header("Audio Listeners")]
    [SerializeField]
    private AudioSource _backgroundAudioSource;
    [SerializeField]
    private AudioSource _uiAudioSource;
    [SerializeField]
    private AudioSource _sfxAudioSource;

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
        _backgroundAudioSource.clip = _map;
        //_backgroundAudioSource.Play();
    }

    private void FadeOutBackground() => _backgroundAudioSource.FadeOut(1f);

    /// <summary>
    /// Finds the area's clip. If it doesn't exist, returns the map clip.
    /// </summary>
    /// <param name="area"> is the area to find the clip for </param>
    /// <returns> the clip for the area. If it doesn't exist, returns the map clip </returns>
    private AudioClip GetAreaClip(Areas area)
    {
        AreaClip clip = _areasClips.Find(a => a.area == area);
        if (clip.clip != null)
        {
            return clip.clip;
        }
        return _map;
    }

    public void StopAll()
    {
        _backgroundAudioSource.Stop();
        _uiAudioSource.Stop();
        _sfxAudioSource.Stop();
    }

    public void PlayBackgroundSound(BackgroundSoundTypes type)
    {
        switch (type)
        {
            case BackgroundSoundTypes.Map:
                StopAll();
                _backgroundAudioSource.clip = _map;
                break;

            case BackgroundSoundTypes.Area:
                FadeOutBackground();
                _backgroundAudioSource.clip = GetAreaClip(Map.Instance.SelectedArea.Area);
                break;

            case BackgroundSoundTypes.ZombiesSpawned:
                FadeOutBackground();
                _backgroundAudioSource.clip = _zombiesSpawned;
                break;
        }
        if (_backgroundAudioSource.clip != null)
        {
            //_backgroundAudioSource.Play();
        }
    }

    public void StopUISound()
    {
        _uiAudioSource.Stop();
    }

    public void PlayUISound(UISoundTypes type)
    {
        switch (type)
        {
            case UISoundTypes.ButtonClick:
                _uiAudioSource.PlayOneShot(_buttonClickSound);
                break;

            case UISoundTypes.Purchase:
                _uiAudioSource.PlayOneShot(_purchaseSound);
                break;
            case UISoundTypes.CoinsCollect:
                _uiAudioSource.PlayOneShot(_coinsCollectSonud);
                break;
        }
    }

    public void PlayUISound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }
        _uiAudioSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }
        _sfxAudioSource.PlayOneShot(clip);
    }

    public void PlayClickSound() => PlayUISound(UISoundTypes.ButtonClick);

}
