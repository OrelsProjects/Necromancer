using UnityEngine;

public class DisableMapMovement : MonoBehaviour
{
    private bool _isFirstEnable = true;

    public virtual void OnEnable()
    {
        if (_isFirstEnable)
        {
            _isFirstEnable = false;
            return;
        }
        if (Map.Instance != null)
        {
            Map.Instance.DisableMovement();
        }
    }

    public virtual void OnDisable()
    {
        if (Map.Instance != null)
        {
            Map.Instance.EnableMovement();
        }

    }
}

