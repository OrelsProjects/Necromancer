using System.Collections;

namespace UnityEngine
{
    public static class AudioSourceExtensions
    {
        public static void FadeOut(this AudioSource a, float duration)
        {
            a.GetComponent<MonoBehaviour>().StartCoroutine(FadeOutCore(a, duration));
        }

        private static IEnumerator FadeOutCore(AudioSource a, float duration)
        {
            float startVolume = a.volume;

            while (a.volume > 0)
            {
                a.volume -= startVolume * Time.deltaTime / duration;
                yield return new WaitForEndOfFrame();
            }

            a.Stop();
            a.volume = startVolume;
        }

        public static void FadeIn(this AudioSource a, float duration)
        {
            a.GetComponent<MonoBehaviour>().StartCoroutine(FadeInCore(a, duration));
        }

        private static IEnumerator FadeInCore(AudioSource a, float duration)
        {
            float startVolume = a.volume;
            a.volume = 0;
            a.Play();

            while (a.volume < startVolume)
            {
                a.volume += startVolume * Time.deltaTime / duration;
                yield return new WaitForEndOfFrame();
            }

            a.volume = startVolume;
        }

         public static void ChangeSoundOverTime(this AudioSource a, float duration, float targeVolume)
        {
            if (a.volume > targeVolume)
            {
                a.GetComponent<MonoBehaviour>().StartCoroutine(ReduceSoundOverTimeCore(a, duration, targeVolume));
            }
            else
            {
                a.GetComponent<MonoBehaviour>().StartCoroutine(IncreaseSoundOverTimeCore(a, duration, targeVolume));
            }
        }

        private static IEnumerator ReduceSoundOverTimeCore(AudioSource a, float duration, float targeVolume)
        {
            float startVolume = a.volume;

            while (a.volume > targeVolume)
            {
                a.volume -= startVolume * Time.deltaTime / duration;
                yield return new WaitForEndOfFrame();
            }

            a.volume = targeVolume;
        }

        private static IEnumerator IncreaseSoundOverTimeCore(AudioSource a, float duration, float targeVolume)
        {
            float startVolume = a.volume;

            while (a.volume < targeVolume)
            {
                a.volume += startVolume * Time.deltaTime / duration;
                yield return new WaitForEndOfFrame();
            }

            a.volume = targeVolume;
        }
    }
}