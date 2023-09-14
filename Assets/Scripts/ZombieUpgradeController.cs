using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeController : MonoBehaviour
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
    private TextMeshProUGUI _nextAttackSpeed;
    [SerializeField]
    private TextMeshProUGUI _nextDamage;
    [SerializeField]
    private TextMeshProUGUI _nextHealth;
    [SerializeField]
    private TextMeshProUGUI _nextSpeed;
    [SerializeField]
    private TextMeshProUGUI _upgradeCost;
    [SerializeField]
    private TextMeshProUGUI _maxLevelText;
    [SerializeField]
    private Image _upgradeCostIcon;
    [SerializeField]
    private Button _upgradeButton;

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
    private ZombieLevel SelectedZombieData
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

    private List<ZombieType> ZombieTypes
    {
        get => InventoryManager.Instance.AcquiredZombies;
    }

    private List<ZombieType> CurrentZombieTypes = new(); // The user might upgrade and get a new zombie, so I need a list that holds the previous zombie types for upgrade for comparison

    private void Start()
    {
    }

    private void OnEnable()
    {
        bool isZombieSelectedForUpgrade = false;

        // Get the difference and add the new zombie to the list via AddZombieOption
        if (CurrentZombieTypes.Count != ZombieTypes.Count)
        {
            List<ZombieType> newZombieTypes = new();
            foreach (ZombieType zombieType in ZombieTypes)
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
                    SelectZombieForUpgrade(uiZombieOption);
                    isZombieSelectedForUpgrade = true;
                }
            }
        }
        UpdateUI();
    }

    private UIZombieOption AddZombieOption(ZombieType zombieType)
    {
        UIZombieOption uiZombieOption = Instantiate(_zombieOptionPrefab);
        Sprite zombieSprite = CharactersManager.Instance.GetZombieSprite(zombieType);
        uiZombieOption.Type = zombieType;
        uiZombieOption.DisableCost();
        uiZombieOption.transform.SetParent(_zombiesContainer);
        uiZombieOption.transform.localScale = new(1.6f, 1.6f);
        uiZombieOption.Image.sprite = zombieSprite;
        uiZombieOption.Button.onClick.AddListener(() => SelectZombieForUpgrade(uiZombieOption));
        CurrentZombieTypes.Add(zombieType);
        return uiZombieOption;
    }

    public void SelectZombieForUpgrade(UIZombieOption zombieOption)
    {
        if (_selectedZombieOption != null)
        {
            EnableZombieOption(_selectedZombieOption, true);
        }
        EnableZombieOption(zombieOption, false);
        _selectedZombieOption = zombieOption;
        Sprite zombieSprite = CharactersManager.Instance.GetZombieSprite(SelectedZombieType.Value);
        _currentZombieImage.sprite = zombieSprite;
        UpdateUI();
    }

    public void Upgrade()
    {
        float upgradeCost = Mathf.Infinity;
        if (SelectedZombieData != null)
        {
            upgradeCost = SelectedZombieData.PriceToUpgrade;
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
        ZombieLevel selectedZombieCurrentLevel = SelectedZombieData;
        _currentAttackSpeed.text = selectedZombieCurrentLevel.AttackSpeed.ToString();
        _currentDamage.text = selectedZombieCurrentLevel.Damage.ToString();
        _currentHealth.text = selectedZombieCurrentLevel.Health.ToString();
        _currentSpeed.text = selectedZombieCurrentLevel.Speed.ToString();
        if (CharactersManager.Instance.IsZombieMaxLevel(SelectedZombieType.Value))
        {
            _maxLevelText.gameObject.SetActive(true);
            _upgradeButton.gameObject.SetActive(false);
            _upgradeCostIcon.enabled = false;

            _nextAttackSpeed.text = STRING_MAX;
            _nextDamage.text = STRING_MAX;
            _nextHealth.text = STRING_MAX;
            _nextSpeed.text = STRING_MAX;
        }
        else
        {
            ZombieLevel selectedZombieNextLevel = CharactersManager.Instance.GetZombieDataNextLevel(SelectedZombieType.Value);

            _maxLevelText.gameObject.SetActive(false);
            _upgradeButton.gameObject.SetActive(true);
            _upgradeCostIcon.enabled = true;

            _upgradeCost.text = selectedZombieCurrentLevel.PriceToUpgrade.ToString();
            _nextAttackSpeed.text = selectedZombieNextLevel.AttackSpeed.ToString();
            _nextDamage.text = selectedZombieNextLevel.Damage.ToString();
            _nextHealth.text = selectedZombieNextLevel.Health.ToString();
            _nextSpeed.text = selectedZombieNextLevel.Speed.ToString();

            if (InventoryManager.Instance.CanAfford(selectedZombieCurrentLevel.PriceToUpgrade))
            {
                _upgradeButton.interactable = true;
            }
            else
            {
                _upgradeButton.interactable = false;
            }
        }
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
