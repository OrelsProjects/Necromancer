using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaUpgradeController : MonoBehaviour {
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI _currentCurrencyPerMinuteText;
    [SerializeField]
    private TextMeshProUGUI _nextCurrencyPerMinuteText;
    [SerializeField]
    private TextMeshProUGUI _upgradeCost;
    [SerializeField]
    private Image _upgradeCostIcon;
    [SerializeField]
    private Button _upgradeButton;

    private Areas area;

    private AreaData _areaData;
    private int CurrentLevel {
        get {
            return AreasManager.Instance.GetAreaLevel(area);
        }
    }

    private void Start() {
        AreasManager.Instance.SubscribeToAreasLevelChange(UpdateUI);
    }

    void OnEnable() {
        _areaData = AreasManager.Instance.GetAreaData(area);
        UpdateUI();
    }

    public void Enable(Areas area) {
        this.area = area;
        gameObject.SetActive(true);
    }

    public void Disable() {
        gameObject.SetActive(false);
    }

    public void Upgrade() {
        AreaLevel level = _areaData.GetAreaLevel(CurrentLevel);
        InventoryManager.Instance.UseCurrency(level.PriceToUpgrade);
        AreasManager.Instance.UpgradeArea(area);
        UpdateUI();
    }

    private void UpdateUI() {
        AreaLevel level = _areaData.GetAreaLevel(CurrentLevel);
        _currentCurrencyPerMinuteText.text = level.CurrencyPerMinute.ToString();
        if (AreasManager.Instance.IsAreaMaxLevel(area)) {
            _upgradeCost.text = "MAXED";
            _upgradeButton.interactable = false;
            _upgradeButton.GetComponent<Image>().enabled = false;
            _upgradeCostIcon.enabled = false;
            return;
        } else {
            _upgradeButton.GetComponent<Image>().enabled = true;
            _upgradeCostIcon.enabled = true;
            _upgradeButton.interactable = true;
        }
        _nextCurrencyPerMinuteText.text = _areaData.GetAreaLevel(CurrentLevel + 1).CurrencyPerMinute.ToString();
        _upgradeCost.text = level.PriceToUpgrade.ToString();

        if (InventoryManager.Instance.CanAfford(level.PriceToUpgrade)) {
            _upgradeButton.interactable = true;
        } else {
            _upgradeButton.interactable = false;
        }
    }
}
