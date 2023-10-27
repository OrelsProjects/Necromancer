using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script handles the fading in and out of ParticleSystems and Text components attached to this GameObject and all its descendants.
/// </summary>
public class FadeInBehaviour : MonoBehaviour
{
    /// <summary>
    /// The time taken for the fade in seconds.
    /// </summary>
    public float fadeTime = 1.5f;

    private void OnEnable()
    {
        // Trigger FadeIn function when the GameObject is enabled.
        FadeIn();
    }

    /// <summary>
    /// Function to initiate fade-in for all child (and their descendants) ParticleSystems and Text components.
    /// </summary>
    public void FadeIn()
    {
        // Reset all opacities to 0 first
        ApplyFadeToChildren(transform, 0.0f);
        // Then fade them in to 1
        ApplyFadeToChildren(transform);
    }

    /// <summary>
    /// Recursively apply fade to all children and their descendants.
    /// </summary>
    /// <param name="parent">The parent whose children will be faded.</param>
    private void ApplyFadeToChildren(Transform parent, float initialAlpha = 0f)
    {
        foreach (Transform child in parent)
        {
            ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
            TMPro.TextMeshProUGUI textComponent = child.GetComponent<TMPro.TextMeshProUGUI>();

            // If this child has a ParticleSystem
            if (particleSystem != null)
            {
                if (initialAlpha >= 0)
                {
                    SetInitialAlpha(particleSystem, initialAlpha);
                }
                StartCoroutine(FadeParticleSystem(particleSystem, 0.0f, 1.0f, fadeTime));
            }

            // If this child has a Text component
            if (textComponent != null)
            {
                if (initialAlpha >= 0)
                {
                    Debug.Log("Settings initial alpha");
                    SetInitialAlpha(textComponent, initialAlpha);
                }
                StartCoroutine(FadeText(textComponent, 0.0f, 1.0f, fadeTime));
            }

            // Recursively apply fade to this child's children
            ApplyFadeToChildren(child, initialAlpha);
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

    private void SetInitialAlpha(TMPro.TextMeshProUGUI textComponent, float alpha)
    {
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);
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
}
