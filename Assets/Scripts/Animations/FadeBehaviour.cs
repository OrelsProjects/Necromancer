using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script handles the fading in and out of ParticleSystems and Text components attached to this GameObject and all its descendants.
/// </summary>
public class FadeBehaviour : MonoBehaviour
{
    public float fadeTime = 1.5f;
    public float delay = 2.0f; // Delay time in seconds
    public bool fadeIn = true;
    public bool fadeOut = false;
    public bool disableOnEnd = false;
    public bool includeSelf = true;

    private void OnEnable()
    {
        if (fadeIn && fadeOut)
        {
            StartCoroutine(FadeInOut());
        }
        else
        {
            if (fadeIn)
            {
                ApplyFade(0.0f, 1.0f);
            }
            else if (fadeOut)
            {
                ApplyFade(1.0f, 0.0f);
            }
        }
        StartCoroutine(DisableAfterFade());
    }

    private IEnumerator FadeInOut()
    {
        ApplyFade(0.0f, 1.0f);
        yield return new WaitForSeconds(fadeTime + delay); // Wait for fadeTime and delay
        ApplyFade(1.0f, 0.0f);
    }

    public void ApplyFade(float startAlpha, float endAlpha)
    {
        if (includeSelf)
        {
            Transform parent = new GameObject("Fade Parent").transform;
            Transform oldParent = transform.parent;
            parent.SetParent(transform.parent);
            ApplyFadeToChildren(transform, startAlpha, endAlpha);
            transform.SetParent(oldParent);
            Destroy(parent.gameObject);
        }
        else
        {
            ApplyFadeToChildren(transform, startAlpha, endAlpha);
        }
    }

    private void ApplyFadeToChildren(Transform parent, float startAlpha, float endAlpha)
    {
        foreach (Transform child in parent)
        {
            ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
            TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();

            if (particleSystem != null)
            {
                SetInitialAlpha(particleSystem, startAlpha);
                StartCoroutine(FadeParticleSystem(particleSystem, startAlpha, endAlpha, fadeTime));
            }

            if (textComponent != null)
            {
                StartCoroutine(FadeText(textComponent, startAlpha, endAlpha, fadeTime));
            }

            ApplyFadeToChildren(child, startAlpha, endAlpha);
        }
    }

    private void SetInitialAlpha(ParticleSystem particleSystem, float alpha)
    {
        ParticleSystem.MainModule mainModule = particleSystem.main;
        Color color = mainModule.startColor.color;
        color.a = alpha;
        ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient(color);
        mainModule.startColor = gradient;
    }


    /// <summary>
    /// Coroutine that handles the fading of a ParticleSystem over a specified time.
    /// </summary>
    /// <param name="particleSystem">The ParticleSystem to fade.</param>
    /// <param name="startAlpha">Initial alpha value.</param>
    /// <param name="endAlpha">Final alpha value.</param>
    /// <param name="time">Time to complete the fade.</param>
    IEnumerator FadeParticleSystem(ParticleSystem particleSystem, float startAlpha, float endAlpha, float time)
    {
        float startTime = Time.time;
        float endTime = Time.time + time;
        ParticleSystem.MainModule mainModule = particleSystem.main;
        Color startColor = mainModule.startColor.color;

        while (Time.time <= endTime)
        {
            float normalizedTime = Mathf.Clamp((Time.time - startTime) / time, 0, 1);
            Color currentColor = Color.Lerp(new Color(startColor.r, startColor.g, startColor.b, startAlpha),
                                            new Color(startColor.r, startColor.g, startColor.b, endAlpha),
                                            normalizedTime);
            ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient(currentColor);
            mainModule.startColor = gradient;
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine that handles the fading of a Text component over a specified time.
    /// </summary>
    /// <param name="textComponent">The Text component to fade.</param>
    /// <param name="startAlpha">Initial alpha value.</param>
    /// <param name="endAlpha">Final alpha value.</param>
    /// <param name="time">Time to complete the fade.</param>
    IEnumerator FadeText(TMPro.TextMeshProUGUI textComponent, float startAlpha, float endAlpha, float time)
    {
        float startTime = Time.time;
        float endTime = Time.time + time;
        Color startColor = textComponent.color;

        while (Time.time <= endTime)
        {
            float normalizedTime = Mathf.Clamp((Time.time - startTime) / time, 0, 1);
            Color currentColor = Color.Lerp(new Color(startColor.r, startColor.g, startColor.b, startAlpha),
                                            new Color(startColor.r, startColor.g, startColor.b, endAlpha),
                                            normalizedTime);
            textComponent.color = currentColor;
            yield return null;
        }
    }

    IEnumerator DisableAfterFade()
    {
        if (disableOnEnd)
        {
            yield return new WaitForSeconds(fadeTime + 0.1f);
            gameObject.SetActive(false);
        }
    }
}
