using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RoundResults : MonoBehaviour
{

    private readonly string _winTitle = "Zombies Won";
    private readonly string _loseTitle = "Defenders Won";

    [Header("UI")]
    [SerializeField]
    private GameObject _reward;
    [SerializeField]
    private GameObject _results;
    [SerializeField]
    private TextMeshProUGUI _currentGoldText;
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _rewardText;
    [SerializeField]
    private Button _continueButton;

    [Header("Sounds")]
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _coinsSound;
    [SerializeField]

    private readonly float _timeToShowReward = 3f;
    private bool _isRoundOver = false;

    private void Start()
    {
        _title.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_isRoundOver)
        {
            return;
        }
        _isRoundOver = RoundManager.Instance.State == RoundState.ZombiesWon || RoundManager.Instance.State == RoundState.DefendersWon;
        if (RoundManager.Instance.State == RoundState.ZombiesWon)
        {
            SetUpLose();
        }
        else if (RoundManager.Instance.State == RoundState.DefendersWon)
        {
            SetUpWin();
        }
    }

    private void OnDisable()
    {
        StopCoroutine(ShowRewardCoroutine());
    }

    private void SetUpWin()
    {
        _rewardText.gameObject.SetActive(true);
        _rewardText.text = RoundManager.Instance.Reward.ToString();
        _title.text = _winTitle;

        StartCoroutine(ShowRewardCoroutine());
        InventoryManager.Instance.AddCurrency(RoundManager.Instance.Reward);
    }

    private void SetUpLose()
    {
        _reward.SetActive(false);
        _rewardText.gameObject.SetActive(false);
        _title.text = _loseTitle;
    }

    private IEnumerator ShowRewardCoroutine()
    {
        yield return new WaitForSeconds(_timeToShowReward);
        ShowReward(true);
    }

    public void ShowReward(bool show)
    {
        _reward.SetActive(show);
    }

    public void ShowResults()
    {
        _results.SetActive(true);
    }

    public void UpdateGoldText()
    {
        float reward = RoundManager.Instance.Reward;
        float currentCurrency = int.Parse(_currentGoldText.text);
        float newCurrency = currentCurrency + reward;
    }
}