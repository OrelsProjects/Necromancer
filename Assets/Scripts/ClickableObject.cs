using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour {
    public UnityEvent onClick;

    private void Awake() {
        onClick ??= new UnityEvent();
    }

    private void OnMouseDown() {
        Debug.Log("On Mouse Down");
        onClick?.Invoke();
    }
}
