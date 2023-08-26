using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



[System.Serializable]
public class ZombieOption {
    public ZombieType Type;
    public GameObject Container;
    public TMPro.TextMeshProUGUI CostText;
}
public class RaidAssembleController : MonoBehaviour {

    public static RaidAssembleController Instance { get; private set; }

    [Header("UI")]
    [SerializeField]
    private GameObject _raidPanel;
    [SerializeField]
    private GameObject _raidButton;
    [SerializeField]
    private TMPro.TextMeshProUGUI _raidPriceText;
    [Tooltip("All possible selected zombies UI placeholders")]
    [SerializeField]
    private List<ZombieSelectPlaceholder> _zombiePlaceholders;
    [Tooltip("All possible zombies UI options")]
    [SerializeField]
    private List<ZombieOption> _zombieOptions;

    private Areas _area;

    private int _raidCost;
    private int RaidCost {
        get {
            return _raidCost;
        }
        set {
            _raidPriceText.text = value.ToString();
            _raidCost = value;
            UpdateRaidButtonInteractable();
        }
    }

    private List<ZombieType> _selectedZombies = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void Raid() {
        bool canRaid = InventoryManager.Instance.UseCurrency(RaidCost);
        if (canRaid) {
            RaidManager.Instance.StartRaid(_area, _selectedZombies);
        } else {
            // TODO: Log it.
            Debug.Log("Something went wrong with raiding.");
        }
    }

    public void ShowRaidPanel(Areas area) {
        Map.Instance.DisableMovement();
        _area = area;
        _raidPanel.SetActive(true);
        ResetValues();
    }

    public void HideRaidPanel() {
        Debug.Log("Hiding: " + _raidPanel);
        Map.Instance.EnableMovement();
        _raidPanel.SetActive(false);
    }

    public void SelectZombie(int index) {
        ZombieOption option = _zombieOptions[index];
        ZombieSelectPlaceholder placeholder = _zombiePlaceholders[index];
        if (option == null) {
            Debug.LogError("ZombieOption is null");
            return;
        }
        if (placeholder == null) {
            Debug.LogError("ZombieSelectPlaceholder is null");
            return;
        }

        if (option.Container.activeSelf) {
            option.Container.SetActive(false);
            if (!placeholder.Container.activeSelf) {
                int priceToUse = CharactersManager.Instance.GetZombieData(option.Type).PriceToUse;
                placeholder.Container.SetActive(true);
                placeholder.SetSelectedZombie(option.Type);
                placeholder.ZombieImage.sprite = CharactersManager.Instance.GetZombieSprite(option.Type);
                placeholder.ZombieCost.text = priceToUse.ToString();
                option.Container.SetActive(false);
                _selectedZombies.Add(option.Type);
                IncraseRaidCost(priceToUse);
                return;
            }
        }
    }

    public void RemoveZombieSelection(int index) {
        ZombieOption option = _zombieOptions[index];
        ZombieSelectPlaceholder placeholder = _zombiePlaceholders[index];
        if (option == null) {
            Debug.LogError("ZombieOption is null");
            return;
        }
        if (placeholder == null) {
            Debug.LogError("ZombieSelectPlaceholder is null");
            return;
        }

        option.Container.SetActive(true);
        placeholder.Container.SetActive(false);
        _selectedZombies.Remove(option.Type);
        DecreaseRaidCost(CharactersManager.Instance.GetZombieData(option.Type).PriceToUse);
    }


    private void ResetValues() {
        _zombieOptions.ForEach(option => {
            ZombieLevel zombieLevel = CharactersManager.Instance.GetZombieData(option.Type);
            option.CostText.text = zombieLevel.PriceToUse.ToString();
            option.Container.SetActive(true);
        });
        _zombiePlaceholders.ForEach(placeholder => placeholder.Container.SetActive(false));
        UpdateRaidCost(AreasManager.Instance.GetAreaData(_area).RaidCost);
    }
    private void UpdateRaidButtonInteractable() {
        if (!InventoryManager.Instance.CanAfford(RaidCost)) {
            _raidButton.GetComponent<Button>().interactable = false;
            return;
        }
        foreach (var placeholder in _zombiePlaceholders) {
            if (placeholder.Container.activeSelf) {
                _raidButton.GetComponent<Button>().interactable = true;
                return;
            }
        }
        _raidButton.GetComponent<Button>().interactable = false;
    }

    private void IncraseRaidCost(int cost) => UpdateRaidCost(RaidCost + cost);
    private void DecreaseRaidCost(int cost) => UpdateRaidCost(RaidCost - cost);

    private void UpdateRaidCost(int cost) {
        RaidCost = cost;
    }
}
