

using System;
using System.Collections.Generic;
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
    [SerializeField]
    private GameObject _addedCurrencyContainer;
    [SerializeField]
    private GameObject _loadingScreen;
    [SerializeField]
    private GameObject _demoCompletedUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        UpdateUI();
        InventoryManager.Instance.OnCurrencyChanged += OnCurrencyChanged;
        OnCurrencyChanged(InventoryManager.Instance.Currency, InventoryManager.Instance.Currency);
        Game.Instance.SubscribeToStateChanged(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnCurrencyChanged -= OnCurrencyChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        UpdateLoading();
    }

    private void OnCurrencyChanged(int newCurrency, int oldCurrency)
    {
        int difference = newCurrency - oldCurrency;
        if (difference != 0)
        {
            string sign = difference > 0 ? "+" : "";
            _currencyText.text = newCurrency.ToString("N0");
            _addedCurrencyContainer.SetActive(false);
            _addedCurrencyContainer.SetActive(true);
            _addedCurrencyContainer.GetComponentInChildren<TextMeshProUGUI>().text = $"{sign}" + difference.ToString("N0");
        }
    }

    public void UpdateUI()
    {
        _currencyText.text = InventoryManager.Instance.Currency.ToString("N0");

        if (AreasManager.Instance.IsGameCompleted())
        {
            AreasManager.Instance.CompleteGame();
            ShowCompletedScreen();
        }

        UpdateLoading();
    }

    private void UpdateLoading()
    {
        if (_loadingScreen == null)
        {
            return;
        }
        if (Game.Instance.State == GameState.Loading)
        {
            _loadingScreen.SetActive(true);
            SoundsManager.Instance.Mute(false);
        }
        else
        {
            _loadingScreen.SetActive(false);
            SoundsManager.Instance.Mute();
        }
    }

    public bool IsUpgradeAreaOpen() => _areaUpgradeUI.GetComponent<AreaUpgradeController>() != null;

    public void ShowZombiesUpgrade()
    {
        _zombiesUpgradeUI.SetActive(true);
    }

    public void HideZombiesUpgrade()
    {
        _zombiesUpgradeUI.SetActive(false);
    }

    public void ShowAreaUpgrade(Areas area)
    {
        _areaUpgradeUI.GetComponent<AreaUpgradeController>().Enable(area);
    }

    public void HideAreaUpgrade()
    {
        _areaUpgradeUI.GetComponent<AreaUpgradeController>().Disable();
    }

    public void LoadPlayground()
    {
        SceneManager.LoadScene("Playground");
    }

    public void PlayClickSound()
    {
        AudioSourceHelper.PlayClipAtPoint(UISoundTypes.ButtonClick);
    }

    public void ShowCompletedScreen()
    {
        _demoCompletedUI.SetActive(true);
    }

    public void HideCompletedScreen()
    {
        _demoCompletedUI.SetActive(false);
    }
}