using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaController : MonoBehaviour
{

    [ShowIf("_state", AreaState.Zombified)]
    [Header("Spawn Point")]
    [SerializeField]
    private Transform _spawnPoint;

    [Header("Area Data")]
    [SerializeField]
    private Areas _area;
    [SerializeField]
    private AreaState _state;

    [Header("UI")]
    [ShowIf("_state", AreaState.Zombified)]
    [SerializeField]
    private GameObject _upgradeButton = null;
    [SerializeField]
    private GameObject _areaTiles;


    [ShowIf("_state", AreaState.Default)]
    [SerializeField]
    private TMPro.TextMeshProUGUI _areaLevelText;


    private GameObject _currentLab;

    private void Start()
    {
        AreasManager.Instance.SubscribeToAreasStateChange(UpdateUI);
        AreasManager.Instance.SubscribeToAreasLevelChange(UpdateUI);
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (AreasManager.Instance == null)
        {
            return;
        }
        AreasManager.Instance.UnsubscribeFromAreasStateChange(UpdateUI);
        AreasManager.Instance.UnsubscribeFromAreasLevelChange(UpdateUI);
    }

    public void ShowUpgrade()
    {
        if (_state == AreaState.Zombified && UIController.Instance.IsUpgradeAreaOpen())
        {
            UIController.Instance.ShowAreaUpgrade(_area);
        }
    }

    public void CloseUpgrade()
    {
        UIController.Instance.HideAreaUpgrade();
    }

    public void Raid() => RaidAssembleController.Instance.ShowRaidPanel(_area);

    public void PlayClickSound() => AudioSourceHelper.PlayClipAtPoint(UISoundTypes.ButtonClick);

    void UpdateUI()
    {
        Dictionary<Areas, AreaState> areasState = AreasManager.Instance.AreasState;
        if (_upgradeButton != null)
        {
            if (AreasManager.Instance.IsAreaMaxLevel(_area))
            {
                _upgradeButton.SetActive(false);
            }
            else
            {
                _upgradeButton.SetActive(true);
            }
        }

        // Update UI
        if (areasState.ContainsKey(_area))
        {
            bool isStateEquals = areasState[_area] == _state; // Does the state in the database equals to the state of this area?
            gameObject.SetActive(isStateEquals);
            _areaTiles.SetActive(isStateEquals);
        }

        if (_state == AreaState.Zombified)
        {
            GameObject newLab = AreasManager.Instance.GetLab(_area);

            if (newLab != null)
            {
                if (_currentLab != null)
                {
                    Destroy(_currentLab);
                }

                _currentLab = Instantiate(AreasManager.Instance.GetLab(_area), transform);
                _currentLab.transform.position = _spawnPoint.position;
                if (_currentLab.TryGetComponent<BoxCollider2D>(out var collider))
                {
                    collider.size = new(3f, 3f);
                }
            }
        }
    }

}