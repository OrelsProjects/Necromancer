using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;


public class AreaController : MonoBehaviour {

    [SerializeField]
    private Areas _area;
    [SerializeField]
    private AreaState _state;
    [ShowIf("_state", AreaState.Zombified)]
    [SerializeField]
    private AreaData _data;

    [Header("UI")]
    [ShowIf("_state", AreaState.Zombified)]
    [SerializeField]
    private GameObject _upgradeButton = null;
    private void Start() {
        AreasManager.Instance.SubscribeToAreasStateChange(UpdateUI);
        UpdateUI(AreasManager.Instance.AreasState);
    }

    private void OnDestroy() {
        if (AreasManager.Instance != null) {
            AreasManager.Instance.UnsubscribeFromAreasStateChange(UpdateUI);
        }
    }

    public void ShowUpgrade() {
        if (_state == AreaState.Zombified) {
            AreasManager.Instance.SelectAreaForUpgrade(_area);
        }
    }

    public void CloseUpgrade() {
        AreasManager.Instance.CloseAreaUpgrade();
    }

    void UpdateUI(Dictionary<Areas, AreaState> _areasState) {
        if (_upgradeButton != null) {
            if (AreasManager.Instance.IsAreaMaxLevel(_area)) {
                _upgradeButton.SetActive(false);
            } else {
                _upgradeButton.SetActive(true);
            }
        }
        if (_areasState.ContainsKey(_area) && _areasState[_area] != _state) {
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
        }
    }
}