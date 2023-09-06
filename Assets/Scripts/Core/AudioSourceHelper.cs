using UnityEngine;

public static class AudioSourceHelper
{
    public static void PlayClipAtPoint(UISoundTypes type, Vector2? position = null)
    {
        AudioClip clip = SoundsManager.GetUISound(type);
        position ??= Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position.Value);
        }
    }

    public static void PlayClipAtPoint(BackgroundSoundTypes type, Vector2 position)
    {
        AudioClip clip = SoundsManager.GetBackgroundSound(type);
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }

    public static void PlayClipAtPoint(Areas area, Vector2 position)
    {
        AudioClip clip = SoundsManager.GetAreaSound(area);
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }
}

