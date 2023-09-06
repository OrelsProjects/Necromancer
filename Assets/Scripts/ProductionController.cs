using System.Collections;
using AssetKits.ParticleImage;
using UnityEngine;
using UnityEngine.UI;

enum ProductionState
{
    NoProduction,
    Production,
    ProductionCollecting,
    Waiting, // Waiting for external click/internal countdown
}

struct MinMax
{
    public float min;
    public float max;
}

public class ProductionController : MonoBehaviour
{

    [SerializeField]
    [Range(1, 60)]
    private int _timeBetweenChecks = 1;

    [Header("UI")]
    [SerializeField]
    private ParticleImage _productionParticles;
    [SerializeField]
    private TMPro.TextMeshProUGUI _currencyText;
    [SerializeField]
    private Image _productionImage;
    [SerializeField]
    private Button _productionButton;

    private ProductionState _state = ProductionState.NoProduction;
    private int _productionToCollect = 0;

    #region Animations
    private MinMax _spawnRate = new() { min = 2f, max = 33f };
    #endregion

    private void Update()
    {
        switch (_state)
        {
            case ProductionState.NoProduction:
                HandleNoProductionState();
                break;
            case ProductionState.Production:
                HandleProductionState();
                break;
            case ProductionState.ProductionCollecting | ProductionState.Waiting:
                break;
        }
    }

    private void HandleNoProductionState()
    {
        _productionToCollect = AreasManager.Instance.CalculateProduction();
        if (_productionToCollect > 0)
        {
            SetState(ProductionState.Production);
            return;
        }

        _productionImage.gameObject.SetActive(false);
        _productionImage.color = new Color(1f, 1f, 1f, 0f);
        SetState(ProductionState.Waiting);
        StartCoroutine(WaitForProduction());
    }

    private void HandleProductionState()
    {
        _productionImage.gameObject.SetActive(true);
        _productionImage.color = new Color(1f, 1f, 1f, 1f);
        _state = ProductionState.Waiting;
    }

    private IEnumerator WaitForProduction()
    {
        yield return new WaitForSeconds(_timeBetweenChecks);
        SetState(ProductionState.NoProduction);
    }

    private void SetState(ProductionState state)
    {
        _state = state;
    }

    public void CollectProduction()
    {
        if (_productionToCollect <= 0 || _state == ProductionState.ProductionCollecting)
        {
            return;
        }
        SetState(ProductionState.ProductionCollecting);
        int maxProduction = AreasManager.Instance.CalculateMaxProduction();
        float currentToMaxProduction = (float)_productionToCollect / maxProduction;
        float newSpawnRate = _spawnRate.min + (_spawnRate.max - _spawnRate.min) * currentToMaxProduction;
        _productionParticles.rateOverTime = newSpawnRate;
        _productionParticles.gameObject.SetActive(true);
        AddProductionToInventory();
    }

    public void AddProductionToInventory()
    {
        StartCoroutine(AddProductionToInventoryCore());
        AudioSourceHelper.PlayClipAtPoint(UISoundTypes.CoinsCollect);
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
        SetState(ProductionState.NoProduction);
        _productionParticles.gameObject.SetActive(false);
        AreasManager.Instance.CollectProduction();
    }
}