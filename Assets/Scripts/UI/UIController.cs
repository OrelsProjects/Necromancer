using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{

    public static UIController Instance;

    [SerializeField]
    private GameObject _zombiesUpgradeUI;
    [SerializeField]
    private GameObject _areaUpgradeUI;
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
        if (_zombiesUpgradeUI.activeSelf)
        {
            Map.Instance.DisableMovement();
        }
        else
        {
            Map.Instance.EnableMovement();
        }
    }

    public bool IsUpgradeAreaOpen() => _areaUpgradeUI.GetComponent<AreaUpgradeController>() != null && _areaUpgradeUI.GetComponent<AreaUpgradeController>().enabled;

    public void ShowAreaUpgrade(Areas area)
    {
        _areaUpgradeUI.GetComponent<AreaUpgradeController>().Enable(area);
        Map.Instance.DisableMovement();
    }

    public void HideAreaUpgrade()
    {
        _areaUpgradeUI.GetComponent<AreaUpgradeController>().Disable();
        Map.Instance.EnableMovement();
    }

    public void LoadPlayground()
    {
        SceneManager.LoadScene("Playground");
    }
}