using System;
using System.Collections;

namespace UnityEngine
{
    public static class AudioSourceExtensions
    {
        public static void FadeOut(this AudioSource a, float duration, Action callback = null)
        {
            if (!a.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
            {
                Debug.LogError("AudioSource must be attached to a MonoBehaviour");
                return;
            }
            monoBehaviour.StartCoroutine(FadeOutCore(a, duration, callback));
        }

        private static IEnumerator FadeOutCore(AudioSource a, float duration, Action callback = null)
        {
            float startVolume = a.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                a.volume = startVolume * (1 - t / duration);
                yield return null;
            }

            a.volume = 0;
            a.Stop();
            callback?.Invoke();
        }

        public static void FadeIn(this AudioSource a, float duration, float targetVolume, Action callback = null)
        {
            if (!a.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
            {
                Debug.LogError("AudioSource must be attached to a MonoBehaviour");
                return;
            }
            a.GetComponent<MonoBehaviour>().StartCoroutine(FadeInCore(a, duration, targetVolume, callback));
        }

        private static IEnumerator FadeInCore(AudioSource a, float duration, float targetVolume, Action callback = null)
        {
            if (!a.isPlaying)
            {
                a.Play();
            }
            float startVolume = a.volume;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                a.volume = Mathf.Lerp(startVolume, targetVolume, t / duration);
                yield return null;
            }

            a.volume = targetVolume;
            callback?.Invoke();
        }

        public static void ChangeSoundOverTime(this AudioSource a, float duration, float targeVolume, Action callback = null)
        {
            if (a.volume > targeVolume)
            {
                a.GetComponent<MonoBehaviour>().StartCoroutine(ReduceSoundOverTimeCore(a, duration, targeVolume, callback));
            }
            else
            {
                a.GetComponent<MonoBehaviour>().StartCoroutine(IncreaseSoundOverTimeCore(a, duration, targeVolume, callback));
            }
        }

        private static IEnumerator ReduceSoundOverTimeCore(AudioSource a, float duration, float targeVolume, Action callback = null)
        {
            float startVolume = a.volume;

            while (a.volume > targeVolume)
            {
                a.volume -= startVolume * Time.deltaTime / duration;
                yield return new WaitForEndOfFrame();
            }

            a.volume = targeVolume;
            callback?.Invoke();
        }

        private static IEnumerator IncreaseSoundOverTimeCore(AudioSource a, float duration, float targeVolume, Action callback = null)
        {
            float startVolume = a.volume;

            while (a.volume < targeVolume)
            {
                a.volume += startVolume * Time.deltaTime / duration;
                yield return new WaitForEndOfFrame();
            }

            a.volume = targeVolume;
            callback?.Invoke();
        }
    }
}