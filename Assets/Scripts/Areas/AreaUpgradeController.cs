using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaUpgradeController : DisableMapMovement
{
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI _currentCurrencyPerMinuteText;
    [SerializeField]
    private TextMeshProUGUI _nextCurrencyPerMinuteText;
    [SerializeField]
    private GameObject _nextStatsContainer;
    [SerializeField]
    private GameObject _nextStatsContainerMax;
    [SerializeField]
    private Image _currentLab;
    [SerializeField]
    private Image _nextLab;
    [SerializeField]
    private TextMeshProUGUI _upgradeCost;
    [SerializeField]
    private Image _upgradeCostIcon;
    [SerializeField]
    private Button _upgradeButton;

    private Areas _area;
    private AreaData _areaData;

    private int CurrentLevel
    {
        get
        {
            return AreasManager.Instance.GetAreaLevel(_area);
        }
    }

    public void Enable(Areas area)
    {
        _area = area;
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        _areaData = AreasManager.Instance.GetAreaData(_area);
        UpdateUI();
    }

    public void Upgrade()
    {
        AreaLevel level = _areaData.GetAreaLevel(CurrentLevel);
        if (InventoryManager.Instance.UseCurrency(level.PriceToUpgrade))
        {
            AreasManager.Instance.UpgradeArea(_area);
            UpdateUI();
        }
    }

    public void HideAreaUpgradeUI() => UIController.Instance.HideAreaUpgrade();

    private void UpdateUI()
    {
        UpdateStats();
        UpdateLabsImages();
    }

    private void UpdateStats()
    {
        AreaLevel level = _areaData.GetAreaLevel(CurrentLevel);
        _currentCurrencyPerMinuteText.text = level.CurrencyPerMinute.ToString();
        if (AreasManager.Instance.IsAreaMaxLevel(_area))
        {
            _upgradeCost.text = "MAX";
            _upgradeButton.interactable = false;

            Image upgradeButtonImage = _upgradeButton.GetComponent<Image>();
            upgradeButtonImage.enabled = false;
            _upgradeCostIcon.enabled = false;
            

            _nextStatsContainer.SetActive(false);
            _nextStatsContainerMax.SetActive(true);

            return;
        }

        _nextStatsContainer.SetActive(true);
        _nextStatsContainerMax.SetActive(false);

        _upgradeButton.GetComponent<Image>().enabled = true;
        _upgradeCostIcon.enabled = true;
        _upgradeButton.interactable = true;

        _nextCurrencyPerMinuteText.text = _areaData.GetAreaLevel(CurrentLevel + 1).CurrencyPerMinute.ToString();
        _upgradeCost.text = level.PriceToUpgrade.ToString();

        if (InventoryManager.Instance.CanAfford(level.PriceToUpgrade))
        {
            _upgradeButton.interactable = true;
        }
        else
        {
            _upgradeButton.interactable = false;
        }
    }

    private void UpdateLabsImages()
    {
        GameObject currentLab = AreasManager.Instance.GetLab(_area);
        GameObject nextLab = AreasManager.Instance.GetNextLab(_area);
        _currentLab.sprite = currentLab.GetComponent<SpriteRenderer>()?.sprite;
        _nextLab.sprite = nextLab.GetComponent<SpriteRenderer>()?.sprite;
    }
}