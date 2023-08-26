using System.Collections;
using AssetKits.ParticleImage;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ProductionController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private ParticleImage _productionParticles;
    [SerializeField]
    private TMPro.TextMeshProUGUI _currencyText;
    [SerializeField]
    private Image _productionImage;
    [SerializeField]
    private Button _productionButton;

    private int _productionToCollect = 0;

    private void Update()
    {
        if (AreasManager.Instance.CalculateProduction() > 0)
        {
            if (!_productionButton.interactable)
            {
                _productionImage.color = new Color(1f, 1f, 1f, 1f);
                _productionButton.interactable = true;
            }
        }
        else if (_productionButton.interactable)
        {
            _productionImage.color = new Color(1f, 1f, 1f, 0f);
            _productionButton.interactable = false;
        }
    }

    public void CollectProduction()
    {
        _productionToCollect = AreasManager.Instance.CalculateProduction();
        if (_productionToCollect <= 0)
        {
            return;
        }
        _productionParticles.gameObject.SetActive(true);
    }

    public void AddProductionToInventory()
    {
        StartCoroutine(AddProductionToInventoryCore());
        SoundsManager.Instance.PlayUISound(UISoundTypes.CoinsCollect);
    }

    public IEnumerator AddProductionToInventoryCore()
    {
        if (_productionToCollect > 0)
        {
            float currentCurrency = InventoryManager.Instance.Currency;
            float newCurrency = currentCurrency + _productionToCollect;
            float timeToUpdate = _productionParticles.duration;
            yield return new WaitForSeconds(1f);
            float elapsedTime = 0f;
            while (elapsedTime < timeToUpdate)
            {
                elapsedTime += Time.deltaTime;
                float currentGold = Mathf.Lerp(currentCurrency, newCurrency, elapsedTime / timeToUpdate);
                _currencyText.text = Mathf.RoundToInt(currentGold).ToString();
                yield return null;
            }
            InventoryManager.Instance.AddCurrency(_productionToCollect);
        }
        _productionToCollect = 0;
        _productionParticles.gameObject.SetActive(false);
        SoundsManager.Instance.StopUISound();
    }
}