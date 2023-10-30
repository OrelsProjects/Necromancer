using UnityEngine;
using UnityEngine.Rendering;

public static class AudioSourceHelper
{
    public static void PlayClipAtPoint(UISoundTypes type, float volume = 1f, Vector2? position = null) =>
        PlayClipAtPoint(SoundsManager.GetUISound(type), position, volume);

    public static void PlayClipAtPoint(BackgroundSoundTypes type, float volume = 1f, Vector2? position = null) =>
        PlayClipAtPoint(SoundsManager.GetBackgroundSound(type), position, volume);

    public static void PlayClipAtPoint(Areas area, float volume = 1f, Vector2? position = null) =>
        PlayClipAtPoint(SoundsManager.GetAreaSound(area), position, volume);

    private static void PlayClipAtPoint(AudioClip clip, Vector2? position = null, float volume = 1f)
    {
        if (clip != null)
        {
            position ??= Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = position.Value;
            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.clip = clip;
            aSource.volume = volume; // Set the volume

            // Play the clip and destroy object after duration of clip
            aSource.Play();
            Object.Destroy(tempGO, clip.length);
        }
    }
}


