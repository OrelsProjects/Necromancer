using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CursorPosition
{
    Above,
    Below,
    Ontop,
    None,
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField]
    private Canvas mainCanvas;

    [SerializeField]
    private GameObject dimMaskPrefab;

    [SerializeField]
    private Image cursorImage; // Assume this Image component displays your cursor sprite
    [SerializeField]
    private GameObject chatContainer;

    [SerializeField]
    [Range(1, 10)]
    private float animationTime = 1.0f; // The time it takes to move the cursor

    private GameObject dimMaskObject;
    private GameObject highlightedObject;
    private GameObject cursorObject;
    private TextMeshProUGUI chatText;

    private Vector3 _previousCursorPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            chatText = chatContainer.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text)
    {
        if (text == "")
        {
            chatContainer.SetActive(false);
        }
        else if (chatText != null)
        {
            chatContainer.SetActive(true);
            chatText.text = text;
        }
    }

    public GameObject HighlightObject(GameObject gameObject)
    {
        if (dimMaskObject != null)
        {
            Destroy(dimMaskObject);
        }
        dimMaskObject = Instantiate(dimMaskPrefab, mainCanvas.transform);
        dimMaskObject.SetActive(true);
        // Make dimMask spread over the entire canvas
        // dimMaskObject.rectTransform.offsetMin = Vector2.zero;
        dimMaskObject.transform.localScale = Vector3.one;

        highlightedObject = Instantiate(gameObject, mainCanvas.transform);
        highlightedObject.SetActive(true);
        highlightedObject.transform.position = gameObject.transform.position;

        SetItemsOrder();
        return highlightedObject;
    }

    private void SetItemsOrder()
    {
        dimMaskObject.transform.SetAsLastSibling();
        chatContainer.transform.SetAsLastSibling();
        highlightedObject.transform.SetAsLastSibling();
    }

    public void ResetHighlight(bool shouldRemoveDim = true)
    {
        if (highlightedObject != null)
        {
            Destroy(highlightedObject);
        }
        if (dimMaskObject != null && shouldRemoveDim)
        {
            Destroy(dimMaskObject.gameObject);
        }
    }

    public void SetCursorPosition(GameObject gameObject, CursorPosition cursorPosition, bool animate = true)
    {
        Vector3 newPosition = gameObject.transform.position;
        if (_previousCursorPosition != null)
        {
            newPosition = _previousCursorPosition;
        }
        if (cursorObject == null)
        {
            cursorObject = Instantiate(cursorImage.gameObject, mainCanvas.transform);
            cursorObject.transform.SetParent(mainCanvas.transform);
        }

        cursorObject.transform.SetAsLastSibling();

        if (gameObject.TryGetComponent(out RectTransform objectRectTransform))
        {
            RectTransform cursorRectTransform = cursorObject.GetComponent<RectTransform>();
            cursorRectTransform.anchorMin = objectRectTransform.anchorMin;
            cursorRectTransform.anchorMax = objectRectTransform.anchorMax;
            cursorRectTransform.anchoredPosition = objectRectTransform.anchoredPosition;
            // cursorRectTransform.sizeDelta = objectRectTransform.sizeDelta;
            float objectHeight = objectRectTransform.rect.height;
            float objectWidth = objectRectTransform.rect.width;
            newPosition = cursorRectTransform.anchoredPosition;
            switch (cursorPosition)
            {
                case CursorPosition.Above:
                    newPosition.y += objectHeight; // 10 units above the object
                    break;
                case CursorPosition.Below:
                    newPosition.y -= objectHeight; // 10 units below the object
                    break;
            }
            newPosition.x -= objectWidth / 2;
        }
        SetCursorPosition(newPosition, animate);
    }

    private void SetCursorPosition(Vector3 newPosition, bool animate = true)
    {
        if (animate)
        {
            StartCoroutine(AnimateCursorPosition(newPosition));
        }
        else
        {
            cursorObject.GetComponent<RectTransform>().anchoredPosition = newPosition;
        }
    }
    private IEnumerator AnimateCursorPosition(Vector3 newPosition)
    {
        RectTransform cursorRectTransform = cursorObject.GetComponent<RectTransform>();
        Vector3 startPosition = cursorRectTransform.anchoredPosition;
        float timeElapsed = 0;

        // Define the springiness and overshoot distance
        float springFactor = 0.1f;
        Vector3 overshootPosition = newPosition + (newPosition - startPosition) * springFactor;

        while (timeElapsed < animationTime)
        {
            if (cursorRectTransform == null)
            {
                yield break; // Stop the coroutine if the cursor object has been destroyed
            }
            float t = timeElapsed / animationTime;

            // Cubic ease-in-out
            float easeInOutT = t < 0.5f ? 4 * Mathf.Pow(t, 3) : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;

            // Use smoothstep for a smoother transition
            float smoothT = easeInOutT * easeInOutT * (3 - 2 * easeInOutT);

            // Main animation phase
            if (t < 0.85f)
            {
                cursorRectTransform.anchoredPosition = Vector3.Lerp(startPosition, overshootPosition, smoothT);
            }
            // Spring back phase
            else
            {
                float springT = (t - 0.85f) / 0.15f;
                float smoothSpringT = springT * springT * (3 - 2 * springT);
                cursorRectTransform.anchoredPosition = Vector3.Lerp(overshootPosition, newPosition, smoothSpringT);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        cursorRectTransform.anchoredPosition = newPosition;
    }

    public void ClearAllObjects()
    {
        Destroy(cursorObject);
        Destroy(dimMaskObject);
        Destroy(highlightedObject);
        chatContainer.SetActive(false);
    }

}