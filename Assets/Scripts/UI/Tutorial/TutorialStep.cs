using UnityEngine;
using UnityEngine.UI;

public class TutorialStep : MonoBehaviour
{
    [SerializeField]
    private TutorialItem item;

    private TutorialManager Manager => TutorialManager.Instance;
    public bool IsComplete => item.isCompleted;
    public int Index => item.index;
    public string Text => item.text;

    private void Update()
    {
        if (item.isCompleted)
        {
            Manager.ClearAllObjects();
            Destroy(this);
        }
        if (item.type == TutorialItemType.PressAnywhere)
        {
            DetectClickAnywhere();
        }
    }

    public void InitiateStep()
    {
        CursorPosition cursorPosition = CursorPosition.Above;
        ActivateStep(cursorPosition, item);
    }

    private void ActivateStep(CursorPosition cursorPosition, TutorialItem item)
    {
        GameObject gameObject = item.gameObject;
        Manager.SetText(Text);
        if (item.isHighlighted)
        {
            gameObject = Manager.HighlightObject(item.gameObject);
        }
        else
        {
            Manager.ResetHighlight();
        }

        switch (item.type)
        {
            case TutorialItemType.Press:
                SetPressObject(gameObject);
                cursorPosition = CursorPosition.Above;
                break;
            case TutorialItemType.Hover:
                cursorPosition = CursorPosition.Ontop;
                break;
            case TutorialItemType.PressAnywhere:
                cursorPosition = CursorPosition.None;
                break;
            default:
                break;
        }
        if (cursorPosition != CursorPosition.None)
        {
            Manager.SetCursorPosition(gameObject, cursorPosition);
        }
    }

    private void SetPressObject(GameObject gameObject)
    {
        bool shouldRemoveButton = false;
        if (item.type == TutorialItemType.PressAnywhere)
        {
            return;
        }
        gameObject.TryGetComponent(out Button button);
        if (button == null)
        {
            button = gameObject.GetComponentInChildren<Button>();
            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
                shouldRemoveButton = true;
            }
        }
        button.onClick.AddListener(() => OnActionCompleted(button, shouldRemoveButton, button.interactable));
        button.interactable = true;
    }

    private void OnActionCompleted(Button button, bool shouldRemoveButton = false, bool interactable = true)
    {
        item.Complete();
        if (shouldRemoveButton)
        {
            Destroy(button);
        }
        else
        {
            button.interactable = interactable;
        }
    }

    public void DetectClickAnywhere()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                item.Complete();
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            item.Complete();
        }
#endif
    }
}