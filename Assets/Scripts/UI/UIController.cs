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

    public bool IsUpgradeAreaOpen() => _areaUpgradeUI.GetComponent<AreaUpgradeController>() != null;

    public void ShowZombiesUpgrade()
    {
        _zombiesUpgradeUI.SetActive(true);
        Map.Instance.DisableMovement();
    }

    public void HideZombiesUpgrade()
    {
        _zombiesUpgradeUI.SetActive(false);
        Map.Instance.EnableMovement();
    }

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

    public void PlayClickSound()
    {
        AudioSourceHelper.PlayClipAtPoint(UISoundTypes.ButtonClick);
    }
}