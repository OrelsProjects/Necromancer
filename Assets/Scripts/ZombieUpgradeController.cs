using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeController : DisableMapMovement
{

    [Header("UI")]
    [SerializeField]
    private UIZombieOption _zombieOptionPrefab;
    [SerializeField]
    private Transform _zombiesContainer;
    [SerializeField]
    private GameObject _arrow;
    [Header("Current")]
    [SerializeField]
    private GameObject _currentStatsContainer;
    [SerializeField]
    private Image _currentZombieImage;
    [SerializeField]
    private TextMeshProUGUI _currentAttackSpeed;
    [SerializeField]
    private TextMeshProUGUI _currentDamage;
    [SerializeField]
    private TextMeshProUGUI _currentHealth;
    [SerializeField]
    private TextMeshProUGUI _currentSpeed;
    [SerializeField]
    private TextMeshProUGUI _currentSpawnAmount;

    [Header("Next")]
    [SerializeField]
    private GameObject _NextStatsContainer;
    [SerializeField]
    private TextMeshProUGUI _nextAttackSpeed;
    [SerializeField]
    private TextMeshProUGUI _nextDamage;
    [SerializeField]
    private TextMeshProUGUI _nextHealth;
    [SerializeField]
    private TextMeshProUGUI _nextSpeed;
    [SerializeField]
    private TextMeshProUGUI _nextSpawnAmount;
    [SerializeField]
    private GameObject _nextSpawnAmountContainer;
    [Header("Upgrade")]
    [SerializeField]
    private TextMeshProUGUI _upgradeCost;
    [SerializeField]
    private Image _upgradeCostIcon;
    [SerializeField]
    private Button _upgradeButton;
    [Header("Acquire")]
    [SerializeField]
    private TextMeshProUGUI _acquireCost;
    [SerializeField]
    private Button _acquireButton;
    [Header("Max Level")]
    [SerializeField]
    private TextMeshProUGUI _maxLevelText;

    private Vector3 _currentStatsOriginalPosition;
    private UIZombieOption _selectedZombieOption;
    private ZombieType? SelectedZombieType
    {
        get
        {
            if (_selectedZombieOption != null)
            {
                return _selectedZombieOption.Type;
            }
            return null;
        }
    }
    private ZombieLevel SelectedZombieLevelData
    {
        get
        {
            if (SelectedZombieType != null)
            {
                return CharactersManager.Instance.GetZombieLevelData(SelectedZombieType.Value);
            }
            return null;
        }
    }

    private ZombieData SelectedZombieData
    {
        get
        {
            if (SelectedZombieType != null)
            {
                return CharactersManager.Instance.GetZombieData(SelectedZombieType.Value);
            }
            return null;
        }
    }

    private List<ZombieType> AcquireableZombies => InventoryManager.Instance.AcquireableZombies;

    private readonly List<ZombieType> CurrentZombieTypes = new(); // Avoid duplicates in the UI.

    public override void OnEnable()
    {
        base.OnEnable();
        _currentStatsOriginalPosition = _currentStatsContainer.transform.position;
        _zombiesContainer.DestroyAllChildren();
        CurrentZombieTypes.Clear();
        bool isZombieSelectedForUpgrade = false;
        if (AcquireableZombies == null || CurrentZombieTypes.Count != AcquireableZombies.Count)
        {
            List<ZombieType> newZombieTypes = new();
            foreach (ZombieType zombieType in AcquireableZombies)
            {
                if (!CurrentZombieTypes.Contains(zombieType))
                {
                    newZombieTypes.Add(zombieType);
                }
            }
            foreach (ZombieType zombieType in newZombieTypes)
            {
                UIZombieOption uiZombieOption = AddZombieOption(zombieType);
                if (!isZombieSelectedForUpgrade)
                {
                    SelectZombie(uiZombieOption);
                    isZombieSelectedForUpgrade = true;
                }
            }
        }
        Map.Instance.DisableMovement();

        UpdateUI();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _currentStatsContainer.transform.position = _currentStatsOriginalPosition;
    }

    private UIZombieOption AddZombieOption(ZombieType zombieType)
    {
        UIZombieOption uiZombieOption = Instantiate(_zombieOptionPrefab);
        ZombieImage zombieImage = CharactersManager.Instance.GetZombieSprite(zombieType).Value;
        uiZombieOption.Type = zombieType;
        uiZombieOption.DisableCost();
        uiZombieOption.transform.SetParent(_zombiesContainer);
        uiZombieOption.transform.localScale = new(1.6f, 1.6f);
        uiZombieOption.Image.sprite = zombieImage.sprite;
        uiZombieOption.Image.transform.localScale = new Vector3(zombieImage.xDim, zombieImage.yDim, 1);
        uiZombieOption.Button.onClick.AddListener(() => SelectZombie(uiZombieOption));
        CurrentZombieTypes.Add(zombieType);
        return uiZombieOption;
    }

    public void SelectZombie(UIZombieOption zombieOption)
    {
        if (_selectedZombieOption != null)
        {
            EnableZombieOption(_selectedZombieOption, true);
        }
        EnableZombieOption(zombieOption, false);
        _selectedZombieOption = zombieOption;
        ZombieImage zombieImage = CharactersManager.Instance.GetZombieSprite(SelectedZombieType.Value).Value;
        _currentZombieImage.sprite = zombieImage.sprite;
        _currentZombieImage.transform.localScale = new Vector3(zombieImage.xDim, zombieImage.yDim, 1);
        UpdateUI();
    }

    public void AcquireZombie()
    {
        if (SelectedZombieData != null)
        {
            if (InventoryManager.Instance.UseCurrency(SelectedZombieData.PriceToAcquire))
            {
                InventoryManager.Instance.AcquireZombie(SelectedZombieType.Value);
                UpdateUI();
            }
        }
    }

    public void Upgrade()
    {
        float upgradeCost = Mathf.Infinity;
        if (SelectedZombieLevelData != null)
        {
            upgradeCost = SelectedZombieLevelData.PriceToUpgrade;
        }
        if (InventoryManager.Instance.UseCurrency((int)upgradeCost))
        {
            CharactersManager.Instance.UpgradeZombie(SelectedZombieType.Value);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (_selectedZombieOption == null)
        {
            return;
        }
        bool isZombieAcquired = InventoryManager.Instance.IsZombieAcquired(_selectedZombieOption.Type);

        ZombieLevel selectedZombieCurrentLevel = SelectedZombieLevelData;
        ZombieData selectedZombieData = SelectedZombieData;
        UpdateText(_currentAttackSpeed, selectedZombieCurrentLevel.AttackSpeed.ToString(), isZombieAcquired);
        UpdateText(_currentDamage, selectedZombieCurrentLevel.Damage.ToString(), isZombieAcquired);
        UpdateText(_currentHealth, selectedZombieCurrentLevel.Health.ToString(), isZombieAcquired);
        UpdateText(_currentSpeed, selectedZombieCurrentLevel.Speed.ToString(), isZombieAcquired);
        UpdateText(_currentSpawnAmount, selectedZombieCurrentLevel.AmountSpawned.ToString(), isZombieAcquired);
        if (CharactersManager.Instance.IsZombieMaxLevel(SelectedZombieType.Value) || !isZombieAcquired)
        {
            CenterCurrentStats();
            EnlargeCurrentStats();

            _upgradeButton.gameObject.SetActive(false);
            _upgradeCost.gameObject.SetActive(false);
            _upgradeCostIcon.gameObject.SetActive(false);
            _arrow.SetActive(false);
            _NextStatsContainer.SetActive(false);

            _maxLevelText.gameObject.SetActive(isZombieAcquired);
            _acquireButton.gameObject.SetActive(!isZombieAcquired);
            if (!isZombieAcquired)
            {
                UpdateText(_acquireCost, selectedZombieData.PriceToAcquire.ToString(), !isZombieAcquired);
            }
        }
        else
        {
            ZombieLevel selectedZombieNextLevel = CharactersManager.Instance.GetZombieDataNextLevel(SelectedZombieType.Value);
            _currentStatsContainer.transform.position = _currentStatsOriginalPosition;
            _currentStatsContainer.transform.localScale = Vector3.one;
            _NextStatsContainer.SetActive(true);
            _maxLevelText.gameObject.SetActive(false);
            _upgradeButton.gameObject.SetActive(true);
            _upgradeCost.gameObject.SetActive(true);
            _upgradeCostIcon.gameObject.SetActive(true);

            _arrow.SetActive(true);
            UpdateText(isZombieAcquired ? _upgradeCost : _acquireCost, selectedZombieCurrentLevel.PriceToUpgrade.ToString(), true);
            UpdateText(_nextAttackSpeed, selectedZombieNextLevel.AttackSpeed.ToString(), isZombieAcquired);
            UpdateText(_nextDamage, selectedZombieNextLevel.Damage.ToString(), isZombieAcquired);
            UpdateText(_nextHealth, selectedZombieNextLevel.Health.ToString(), isZombieAcquired);
            UpdateText(_nextSpeed, selectedZombieNextLevel.Speed.ToString(), isZombieAcquired);
            UpdateText(_nextSpawnAmount, selectedZombieNextLevel.AmountSpawned.ToString(), isZombieAcquired);
            _nextSpawnAmountContainer.SetActive(true);

            if (isZombieAcquired)
            {
                bool canAfford = InventoryManager.Instance.CanAfford(selectedZombieCurrentLevel.PriceToUpgrade);
                _upgradeButton.interactable = canAfford;
            }
            else
            {
                bool canAfford = InventoryManager.Instance.CanAfford(selectedZombieCurrentLevel.PriceToUpgrade);
                _acquireButton.interactable = canAfford;
            }
        }

        _acquireButton.gameObject.SetActive(!isZombieAcquired);
    }

    private void CenterCurrentStats() => _currentStatsContainer.transform.localPosition = new Vector3(-85f, -85f, 0);
    private void EnlargeCurrentStats() => _currentStatsContainer.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

    private void UpdateText(TextMeshProUGUI text, string value, bool isZombieAcquired = false)
    {
        text.text = value;
        text.alpha = isZombieAcquired ? 1 : 0.45f;
    }

    private void EnableZombieOption(UIZombieOption zombieOption, bool isEnabled)
    {
        if (!isEnabled)
        {
            zombieOption.Button.interactable = false;
            zombieOption.Image.color = new Color(1, 1, 1, 0.45f);
        }
        else
        {
            zombieOption.Button.interactable = true;
            zombieOption.Image.color = new Color(1, 1, 1, 1);
        }
    }
}
