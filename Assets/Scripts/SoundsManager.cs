using UnityEngine;

public enum SoundTypes
{
    BackgroundMusic,
    ButtonClick
}

public class SoundsManager : MonoBehaviour
{

    public static SoundsManager Instance;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip _backgroundMusic;
    [SerializeField]
    private AudioClip _buttonClickSound;


    [Header("Audio Listeners")]
    [SerializeField]
    private AudioSource _stepsAudioSource;
    [SerializeField]
    private AudioSource _backgroundAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _backgroundAudioSource.clip = _backgroundMusic;
        _backgroundAudioSource.loop = true;
        _backgroundAudioSource.Play();
    }

    public void PlayStepSound(AudioClip clip)
    {
        _stepsAudioSource.PlayOneShot(clip);
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        AudioClip clipToPlay = _backgroundMusic;
        if (clip != null)
        {
            clipToPlay = clip;
        }
        _backgroundAudioSource.clip = _backgroundMusic;
        _backgroundAudioSource.loop = true;
        _backgroundAudioSource.Play();
    }


    public void PlayBackgroundMusicFadeIn(AudioClip clip, float duration, float volume = 100)
    {
        AudioClip clipToPlay = _backgroundMusic;
        if (clip != null)
        {
            clipToPlay = clip;
        }
        _backgroundAudioSource.volume = volume;
        _backgroundAudioSource.clip = _backgroundMusic;
        _backgroundAudioSource.loop = true;
        _backgroundAudioSource.FadeIn(duration);
    }

    public void StopBackgroundMusic()
    {
        _backgroundAudioSource.Stop();
    }

    public void StopBackgroundMusicFadeOut(float duration)
    {
        //_backgroundAudioSource.FadeOut(duration);
    }

    public void PlayPressSound()
    {
        PlaySound(SoundTypes.ButtonClick);
    }

    public void PlaySound(SoundTypes sounds)
    {
        switch (sounds)
        {
            case SoundTypes.BackgroundMusic:
                _backgroundAudioSource.Play();
                break;
            case SoundTypes.ButtonClick:
                _backgroundAudioSource.PlayOneShot(_buttonClickSound);
                break;
        }
    }
}
