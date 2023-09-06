using Sirenix.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeController : MonoBehaviour
{

    [Header("Zombies Placeholders")]
    [SerializeField]
    private List<ZombieSelectPlaceholder> _zombiePlaceholders;
    [Header("Zombie Types")]
    [SerializeField]
    private List<ZombieType> _zombieTypes;

    [Header("UI")]
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

    private ZombieType _selectedZombieType = ZombieType.ZombieLab;
    private int _selectedIndex = 0;

    private void Start()
    {
        int index = 0;
        _zombieTypes.ForEach(zombieType =>
        {
            ZombieSelectPlaceholder zombiePlaceholder = _zombiePlaceholders[index++];
            if (zombiePlaceholder == null)
            {
                Debug.LogError("Zombie placeholder is null");
                return;
            }
            Sprite zombieSprite = CharactersManager.Instance.GetZombieSprite(zombieType);
            if (zombieSprite == null)
            {
                Debug.LogError("Zombie sprite is null");
                return;
            }
            zombiePlaceholder.Container.SetActive(true);
            zombiePlaceholder.ZombieImage.sprite = zombieSprite;
        });
        SelectZombieForUpgrade(0);
    }

    void OnEnable()
    {
        UpdateUI();
    }

    public void SelectZombieForUpgrade(int index)
    {
        _selectedZombieType = _zombieTypes[index];
        Debug.Log("Selected zombie type: " + _selectedZombieType);
        _selectedIndex = index;
        UpdatePlaceholders();
        UpdateUI();
    }

    public void Upgrade()
    {
        int upgradeCost = CharactersManager.Instance.GetZombieData(_selectedZombieType)?.PriceToUpgrade ?? 99999999;
        // TODO: Log if upgradeCost is bad.
        if (InventoryManager.Instance.UseCurrency(upgradeCost))
        {
            CharactersManager.Instance.UpgradeZombie(_selectedZombieType);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        ZombieLevel selectedZombieCurrentLevel = CharactersManager.Instance.GetZombieData(_selectedZombieType);
        _currentAttackSpeed.text = selectedZombieCurrentLevel.AttackSpeed.ToString();
        _currentDamage.text = selectedZombieCurrentLevel.Damage.ToString();
        _currentHealth.text = selectedZombieCurrentLevel.Health.ToString();
        _currentSpeed.text = selectedZombieCurrentLevel.Speed.ToString();
        if (CharactersManager.Instance.IsZombieMaxLevel(_selectedZombieType))
        {
            string maxString = "MAX";

            _maxLevelText.gameObject.SetActive(true);
            _upgradeButton.gameObject.SetActive(false);
            _upgradeCostIcon.enabled = false;

            _nextAttackSpeed.text = maxString;
            _nextDamage.text = maxString;
            _nextHealth.text = maxString;
            _nextSpeed.text = maxString;
        }
        else
        {
            ZombieLevel selectedZombieNextLevel = CharactersManager.Instance.GetZombieDataNextLevel(_selectedZombieType);

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

    private void UpdatePlaceholders()
    {
        Sprite zombieSprite = CharactersManager.Instance.GetZombieSprite(_selectedZombieType);
        _currentZombieImage.sprite = zombieSprite;
        for (int i = 0; i < _zombiePlaceholders.Count; i += 1)
        {
            ZombieSelectPlaceholder placeholder = _zombiePlaceholders[i];
            if (i == _selectedIndex)
            {
                placeholder.Button.interactable = false;
                placeholder.Container.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                placeholder.ZombieImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                placeholder.Button.interactable = true;
                placeholder.Container.GetComponent<Image>().color = new Color(1, 1, 1, 0.45f);
                placeholder.ZombieImage.color = new Color(1, 1, 1, 0.45f);
            }
        }
    }
}
