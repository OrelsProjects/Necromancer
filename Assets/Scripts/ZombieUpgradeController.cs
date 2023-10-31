using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeController : DisableMapMovement
{

    const string STRING_MAX = "MAX";

    [Header("UI")]
    [SerializeField]
    private UIZombieOption _zombieOptionPrefab;
    [SerializeField]
    private Transform _zombiesContainer;
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
    [SerializeField]
    private TextMeshProUGUI _upgradeCost;
    [SerializeField]
    private TextMeshProUGUI _acquireCost;

    [SerializeField]
    private TextMeshProUGUI _maxLevelText;
    [SerializeField]
    private Image _upgradeCostIcon;
    [SerializeField]
    private Button _upgradeButton;
    [SerializeField]
    private Button _acquireButton;

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
        _zombiesContainer.DestroyAllChildren();
        CurrentZombieTypes.Clear();
        bool isZombieSelectedForUpgrade = false;
        Debug.Log("AcquireableZombies.Count: " + AcquireableZombies.Count);
        if (CurrentZombieTypes.Count != AcquireableZombies.Count)
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
        Debug.Log("Price To Acquire: " + SelectedZombieData.PriceToAcquire);
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
        UpdateText(_currentAttackSpeed, selectedZombieCurrentLevel.AttackSpeed.ToString(), isZombieAcquired);
        UpdateText(_currentDamage, selectedZombieCurrentLevel.Damage.ToString(), isZombieAcquired);
        UpdateText(_currentHealth, selectedZombieCurrentLevel.Health.ToString(), isZombieAcquired);
        UpdateText(_currentSpeed, selectedZombieCurrentLevel.Speed.ToString(), isZombieAcquired);
        UpdateText(_currentSpawnAmount, selectedZombieCurrentLevel.AmountSpawned.ToString(), isZombieAcquired);
        if (CharactersManager.Instance.IsZombieMaxLevel(SelectedZombieType.Value))
        {
            _maxLevelText.gameObject.SetActive(true);
            _upgradeButton.gameObject.SetActive(false);
            _upgradeCostIcon.enabled = false;

            _nextAttackSpeed.text = STRING_MAX;
            _nextDamage.text = STRING_MAX;
            _nextHealth.text = STRING_MAX;
            _nextSpeed.text = STRING_MAX;
            _nextSpawnAmountContainer.SetActive(false);
        }
        else
        {
            ZombieLevel selectedZombieNextLevel = CharactersManager.Instance.GetZombieDataNextLevel(SelectedZombieType.Value);

            _maxLevelText.gameObject.SetActive(false);
            _upgradeButton.gameObject.SetActive(true);
            _upgradeCostIcon.enabled = true;
            if (isZombieAcquired)
            {
                UpdateText(_upgradeCost, selectedZombieCurrentLevel.PriceToUpgrade.ToString(), true);
            }
            else
            {
                UpdateText(_acquireCost, SelectedZombieData.PriceToAcquire.ToString(), true);
            }
            UpdateText(_nextAttackSpeed, selectedZombieNextLevel.AttackSpeed.ToString(), isZombieAcquired);
            UpdateText(_nextDamage, selectedZombieNextLevel.Damage.ToString(), isZombieAcquired);
            UpdateText(_nextHealth, selectedZombieNextLevel.Health.ToString(), isZombieAcquired);
            UpdateText(_nextSpeed, selectedZombieNextLevel.Speed.ToString(), isZombieAcquired);
            UpdateText(_nextSpawnAmount, selectedZombieNextLevel.AmountSpawned.ToString(), isZombieAcquired);
            _nextSpawnAmountContainer.SetActive(true);

            _upgradeButton.gameObject.SetActive(isZombieAcquired);
            _acquireButton.gameObject.SetActive(!isZombieAcquired);

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
    }

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
