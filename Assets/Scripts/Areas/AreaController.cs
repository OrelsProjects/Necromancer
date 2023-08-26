using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [ShowIf("_state", AreaState.Zombified)]
    [SerializeField]
    private AreaData _data;

    [Header("UI")]
    [ShowIf("_state", AreaState.Zombified)]
    [SerializeField]
    private GameObject _upgradeButton = null;


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
        if (areasState.ContainsKey(_area) && areasState[_area] != _state)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
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
                if (_currentLab.TryGetComponent<ClickableObject>(out var clickable))
                {
                    clickable.onClick.AddListener(ShowUpgrade);
                }
            }
        }
    }
}