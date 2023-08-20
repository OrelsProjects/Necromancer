using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour {
    public UnityEvent onClick;

    private void Awake() {
        InitCollider();
    }

    private void InitCollider() {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new(0.6f, 0.6f);
    }

    private void OnMouseDown() => onClick?.Invoke();

}
