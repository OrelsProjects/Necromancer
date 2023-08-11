using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class AreaController : MonoBehaviour
{

    [SerializeField]
    private Areas _area;
    [SerializeField]
    private AreaState _state;
    [ShowIf("_state", AreaState.Zombified)]
    [SerializeField]
    private AreaData _data;

    void Awake()
    {
        AreasManager.Instance.OnAreaStateChanged += UpdateUI;
    }

    private void Start()
    {
        UpdateUI(AreasManager.Instance.AreasState);
    }

    private void OnDestroy()
    {
        if (AreasManager.Instance != null)
        {
            AreasManager.Instance.OnAreaStateChanged -= UpdateUI;
        }
    }

    public void ShowUpgrade()
    {
        if (_state == AreaState.Zombified)
        {
            AreasManager.Instance.SelectAreaForUpgrade(_area);
        }
    }

    public void CloseUpgrade()
    {
        AreasManager.Instance.CloseAreaUpgrade();
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