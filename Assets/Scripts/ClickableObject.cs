using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour {
    public UnityEvent onClick;

    public bool interactable = true;

    private void Awake() {
        InitCollider();
    }

    private void InitCollider() {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new(0.6f, 0.6f);
    }

    private void OnMouseDown() {
        if (interactable) {
            onClick.Invoke();
        }
    }

    // function that allows any type of action to be added to the event
    public void AddClickEvent(UnityAction action) {
        onClick ??= new();
        onClick.AddListener(action);
    }

}
