using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{

    public static UIController Instance;

    [SerializeField]
    private GameObject _zombiesUpgradeUI;
    [SerializeField]
    private TextMeshProUGUI _currencyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        InventoryManager.Instance.OnCurrencyChanged += OnCurrencyChanged;

        OnCurrencyChanged(InventoryManager.Instance.Currency);
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnCurrencyChanged -= OnCurrencyChanged;
    }
    private void OnCurrencyChanged(int newCurrency)
    {
        _currencyText.text = newCurrency.ToString();
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        _currencyText.text = InventoryManager.Instance.Currency.ToString();
    }

    public void ShowZombiesUpgrade()
    {
        _zombiesUpgradeUI.SetActive(!_zombiesUpgradeUI.activeSelf);
    }

    public void PlayButtonClickSound()
    {
        SoundsManager.Instance.PlaySound(SoundTypes.ButtonClick);
    }
}