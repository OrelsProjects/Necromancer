using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



[Serializable]
public class ZombieOption
{
    public ZombieType Type;
    public GameObject Container;
    public TMPro.TextMeshProUGUI CostText;
}
public class RaidAssembleController : DisableMapMovement
{

    public static RaidAssembleController Instance { get; private set; }

    [Header("UI")]
    [SerializeField]
    private GameObject _raidPanel;
    [SerializeField]
    private GameObject _raidButton;
    [SerializeField]
    private UIZombieOption _zombieOptionPrefab;
    [SerializeField]
    private Transform _optionsList;
    [SerializeField]
    private Transform _selectedList;
    [SerializeField]
    private TMPro.TextMeshProUGUI _raidPriceText;

    private Dictionary<ZombieType, UIZombieOption> _zombieOptions = new();

    private Areas _area;

    private int _raidCost;
    private int RaidCost
    {
        get
        {
            return _raidCost;
        }
        set
        {
            _raidCost = value;
            _raidPriceText.text = _raidCost.ToString();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitOptionsList()
    {
        foreach (Transform child in _optionsList)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _selectedList)
        {
            Destroy(child.gameObject);
        }

        _zombieOptions = new();
        int position = 0;
        foreach (ZombieType zombieType in InventoryManager.Instance.AcquiredZombies)
        {
            UIZombieOption raidZombieOption = GetRaidZombieUI(zombieType, position++);
            _zombieOptions.Add(zombieType, raidZombieOption);
            AddToOptionsList(raidZombieOption);
        }
        UpdateRaidCost(AreasManager.Instance.GetAreaData(_area).RaidCost);
        UpdateRaidButtonInteractable();
    }

    private UIZombieOption GetRaidZombieUI(ZombieType zombieType, int positionInList = 0)
    {
        UIZombieOption raidZombieOption = Instantiate(_zombieOptionPrefab);
        ZombieImage zombieImage = CharactersManager.Instance.GetZombieSprite(zombieType).Value;
        int cost = CharactersManager.Instance.GetZombieLevelData(zombieType).PriceToUse;

        raidZombieOption.Type = zombieType;
        raidZombieOption.Image.sprite = zombieImage.sprite;
        raidZombieOption.Image.transform.localScale = new Vector3(zombieImage.xDim, zombieImage.yDim, 1);
        raidZombieOption.CostText.text = cost.ToString();
        raidZombieOption.PositionInList = positionInList;
        raidZombieOption.Button.onClick.AddListener(() => SelectZombie(raidZombieOption));
        return raidZombieOption;
    }

    private void AddToOptionsList(UIZombieOption raidZombieOption)
    {
        raidZombieOption.transform.SetParent(_optionsList);
        raidZombieOption.transform.localPosition = new(raidZombieOption.PositionInList, raidZombieOption.PositionInList);
        raidZombieOption.transform.localScale = Vector2.one;
    }

    private void AddToSelectedList(UIZombieOption raidZombieOption)
    {
        raidZombieOption.transform.SetParent(_selectedList);
        raidZombieOption.transform.localScale = Vector2.one;
    }

    private void SelectZombie(UIZombieOption raidZombieOption)
    {
        raidZombieOption.ResetButtonListeners();
        raidZombieOption.Button.onClick.AddListener(() => RemoveZombieSelection(raidZombieOption));
        IncreaseRaidCost(CharactersManager.Instance.GetZombieLevelData(raidZombieOption.Type).PriceToUse);
        AddToSelectedList(raidZombieOption);
        UpdateRaidButtonInteractable();
    }

    private void RemoveZombieSelection(UIZombieOption raidZombieOption)
    {
        raidZombieOption.ResetButtonListeners();
        raidZombieOption.Button.onClick.AddListener(() => SelectZombie(raidZombieOption));
        DecreaseRaidCost(CharactersManager.Instance.GetZombieLevelData(raidZombieOption.Type).PriceToUse);
        AddToOptionsList(raidZombieOption);
        UpdateRaidButtonInteractable();
    }

    public void SelectZombie(ZombieType zombieType)
    {
        if (!_zombieOptions.ContainsKey(zombieType))
        {
            return;
        }
        UIZombieOption raidZombieOption = _zombieOptions[zombieType];
        SelectZombie(raidZombieOption);
    }

    public void RemoveZombieSelection(ZombieType zombieType)
    {
        if (!_zombieOptions.ContainsKey(zombieType))
        {
            return;
        }
        UIZombieOption raidZombieOption = _zombieOptions[zombieType];
        RemoveZombieSelection(raidZombieOption);
    }

    public void Raid()
    {
        List<ZombieType> selectedZombies = new();
        for (int i = 0; i < _selectedList.childCount; i++)
        {
            ZombieType selectedZombieType = _selectedList.GetChild(i).GetComponent<UIZombieOption>().Type;
            selectedZombies.Add(selectedZombieType);
        }

        bool canRaid = InventoryManager.Instance.CanAfford(RaidCost) && selectedZombies.Count > 0;
        if (canRaid)
        {
            RaidManager.Instance.StartRaid(_area, selectedZombies, RaidCost);
        }
        else
        {
            // TODO: Log it.
            Debug.Log("Can't Afford Raid");
        }
    }

    public void ShowRaidPanel(Areas area)
    {
        Map.Instance.DisableMovement();
        _area = area;
        _raidPanel.SetActive(true);
        InitOptionsList();
    }

    public void HideRaidPanel()
    {
        Map.Instance.EnableMovement();
        _raidPanel.SetActive(false);
    }

    private void UpdateRaidButtonInteractable() =>
        _raidButton.GetComponent<Button>().interactable = InventoryManager.Instance.CanAfford(RaidCost) && _selectedList.childCount > 0;

    private void IncreaseRaidCost(int cost) => UpdateRaidCost(RaidCost + cost, "Increase");
    private void DecreaseRaidCost(int cost) => UpdateRaidCost(RaidCost - cost, "Decrease");

    private void UpdateRaidCost(int cost, string caller = "Other") => RaidCost = cost;

}
