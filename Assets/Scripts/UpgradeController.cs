using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeController : MonoBehaviour
{
    [Header("UI")]
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
    private Button _upgradeButton;

    private ZombieType _selectedZombieType = ZombieType.ZombieLab;

    void OnEnable()
    {
        UpdateUI();
    }

    public void Upgrade()
    {
        int upgradeCost = CharactersManager.Instance.GetZombieData(_selectedZombieType)?.PriceToUpgrade ?? 99999999;
        // TODO: Log if upgradeCost is bad.
        InventoryManager.Instance.UseCurrency(upgradeCost);
        CharactersManager.Instance.UpgradeZombie(_selectedZombieType);
        UpdateUI();
    }

    private void UpdateUI()
    {
        ZombieLevel selectedZombieCurrentLevel = CharactersManager.Instance.GetZombieData(_selectedZombieType);
        _currentAttackSpeed.text = selectedZombieCurrentLevel.AttackSpeed.ToString();
        _currentDamage.text = selectedZombieCurrentLevel.Damage.ToString();
        _currentHealth.text = selectedZombieCurrentLevel.Health.ToString();
        _currentSpeed.text = selectedZombieCurrentLevel.Speed.ToString();
        _upgradeCost.text = selectedZombieCurrentLevel.PriceToUpgrade.ToString();

        ZombieLevel selectedZombieNextLevel = CharactersManager.Instance.GetZombieDataNextLevel(_selectedZombieType);
        _nextAttackSpeed.text = selectedZombieNextLevel.AttackSpeed.ToString();
        _nextDamage.text = selectedZombieNextLevel.Damage.ToString();
        _nextHealth.text = selectedZombieNextLevel.Health.ToString();
        _nextSpeed.text = selectedZombieNextLevel.Speed.ToString();

        if (InventoryManager.Instance.CanAfford(selectedZombieCurrentLevel.PriceToUpgrade))
        {
            _upgradeButton.enabled = true;
        }
        else
        {
            _upgradeButton.enabled = false;
        }
    }
}
