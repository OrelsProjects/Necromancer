using TMPro;
using UnityEngine;

public class ZombieCard : MonoBehaviour
{
    [SerializeField]
    private ZombieCardData _data;

    [Header("UI Elements")]
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private TextMeshProUGUI _healthText;
    [SerializeField]
    private TextMeshProUGUI _speedText;
    [SerializeField]
    private TextMeshProUGUI _damageText;
    [SerializeField]
    private TextMeshProUGUI _costText;
    [SerializeField]
    private TextMeshProUGUI _amountText;


    private void Awake()
    {
        _nameText.text = _data.Name;
        _healthText.text = _data.Health.ToString();
        _speedText.text = _data.Speed.ToString();
        _damageText.text = _data.Power.ToString();
        _costText.text = _data.Cost.ToString();
        _amountText.text = 0.ToString();
    }

    public void IncreaseAmount(int amount)
    {
        Debug.Log("IncreaseAmount");
    }

    public void DecreaseAmount(int amount)
    {
        Debug.Log("DecreaseAmount");
    }
}