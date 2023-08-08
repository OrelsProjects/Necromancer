using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AreaController : MonoBehaviour
{

    [SerializeField]
    private Areas _area;
    [SerializeField]
    private AreaState _state;

    void Start()
    {
        AreasManager.Instance.OnAreaStateChanged += UpdateUI;
        UpdateUI(AreasManager.Instance.AreasState);
    }

    private void OnDestroy()
    {
        if (AreasManager.Instance != null)
        {
            AreasManager.Instance.OnAreaStateChanged -= UpdateUI;
        }
    }

    void UpdateUI(Dictionary<Areas, AreaState> _areasState)
    {
        if (_areasState.ContainsKey(_area) && _areasState[_area] != _state)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}