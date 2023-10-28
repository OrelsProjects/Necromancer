using System.Collections;
using UnityEngine;

/// <summary>
/// This script handles the floating behavior of GameObjects and all their descendants.
/// </summary>
public class FloatBehaviour : MonoBehaviour
{
    public float floatTime = 1.5f;
    public float delay = 2.0f; // Delay time in seconds
    public bool floatUp = true;
    public bool floatDown = false;
    public bool includeSelf = true;
    public float floatDistance = 1.0f; // Distance to float up or down

    private Vector3 _objectInitialPosition;
    private Transform _objectToFloat;

    private void OnEnable()
    {
        if (floatUp && floatDown)
        {
            StartCoroutine(FloatUpDown());
        }
        else if (floatUp)
        {
            ApplyFloat(0.0f, floatDistance);
        }
        else if (floatDown)
        {
            ApplyFloat(0.0f, -floatDistance);
        }
    }

    private IEnumerator FloatUpDown()
    {
        ApplyFloat(0.0f, floatDistance, false);
        yield return new WaitForSeconds(floatTime + delay);
        ApplyFloat(0.0f, -floatDistance);
    }

    public void ApplyFloat(float startPosition, float endPosition, bool returnToInitialPosition = true)
    {
        if (includeSelf)
        {
            _objectToFloat = transform;
            StartCoroutine(FloatObject(startPosition, endPosition, floatTime, returnToInitialPosition));
        }
        ApplyFloatToChildren(transform, startPosition, endPosition);
    }

    private void ApplyFloatToChildren(Transform parent, float startPosition, float endPosition)
    {
        foreach (Transform child in parent)
        {
            _objectToFloat = child;
            StartCoroutine(FloatObject(startPosition, endPosition, floatTime));
            ApplyFloatToChildren(child, startPosition, endPosition);
        }
    }

    /// <summary>
    /// Coroutine that handles the floating of an object over a specified time.
    /// </summary>
    /// <param name="objectToFloat">The object to float.</param>
    /// <param name="startPosition">Initial position relative to current position.</param>
    /// <param name="endPosition">Final position relative to current position.</param>
    /// <param name="time">Time to complete the float.</param>
    IEnumerator FloatObject(float startPosition, float endPosition, float time, bool returnToInitialPosition = true)
    {
        float startTime = Time.time;
        float endTime = Time.time + time;
        _objectInitialPosition = _objectToFloat.position;

        while (Time.time <= endTime)
        {
            float normalizedTime = Mathf.Clamp((Time.time - startTime) / time, 0, 1);
            float currentY = Mathf.Lerp(_objectInitialPosition.y + startPosition, _objectInitialPosition.y + endPosition, normalizedTime);
            _objectToFloat.position = new Vector3(_objectInitialPosition.x, currentY, _objectInitialPosition.z);
            yield return null;
        }
        if (returnToInitialPosition)
        {
            _objectToFloat.position = _objectInitialPosition;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _objectToFloat.position = _objectInitialPosition;
    }
}
