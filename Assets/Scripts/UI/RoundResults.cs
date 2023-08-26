using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AssetKits.ParticleImage;
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
    private float _coinsSoundDelay = 0f;

    [Header("Settings")]
    [SerializeField]
    private ParticleImage _rewardParticles;

    private readonly float _timeToShowReward = 3f;
    private bool _isRoundOver = false;

    private void Start()
    {
        _title.gameObject.SetActive(true);
        _currentGoldText.text = InventoryManager.Instance.Currency.ToString();
        _continueButton.onClick.AddListener(() => FinishRound());
    }

    private void Update()
    {
        _isRoundOver = RoundManager.Instance.State == RoundState.Lost || RoundManager.Instance.State == RoundState.Won;
        if (RoundManager.Instance.State == RoundState.Lost)
        {
            SetUpLose();
        }
        else if (RoundManager.Instance.State == RoundState.Won)
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

        _title.GetComponent<Animator>().SetBool("Win", true);
        _title.GetComponent<Animator>().SetBool("Lose", false);

        StartCoroutine(ShowRewardCoroutine());
        InventoryManager.Instance.AddCurrency(RoundManager.Instance.Reward);
    }

    private void SetUpLose()
    {
        _reward.SetActive(false);
        _rewardText.gameObject.SetActive(false);
        _title.text = _loseTitle;

        _title.GetComponent<Animator>().SetBool("Win", false);
        _title.GetComponent<Animator>().SetBool("Lose", true);
    }

    private void FinishRound()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
        RoundManager.Instance.FinishRound();
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

    public void PlayCoinsSound()
    {
        StartCoroutine(PlaySoundWithDelay(_coinsSound, _coinsSoundDelay));
    }

    public void UpdateGoldText()
    {
        float reward = RoundManager.Instance.Reward;
        float currentCurrency = int.Parse(_currentGoldText.text);
        float newCurrency = currentCurrency + reward;
        float timeToUpdate = _rewardParticles.lifetime.constant + 0.3f;
        StartCoroutine(UpdateGoldTextCoroutine(currentCurrency, newCurrency, timeToUpdate));
    }

    private IEnumerator UpdateGoldTextCoroutine(float currentCurrency, float newCurrency, float timeToUpdate)
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeToUpdate)
        {
            elapsedTime += Time.deltaTime;
            float currentGold = Mathf.Lerp(currentCurrency, newCurrency, elapsedTime / timeToUpdate);
            _currentGoldText.text = Mathf.RoundToInt(currentGold).ToString();
            yield return null;
        }
        _currentGoldText.text = Mathf.RoundToInt(newCurrency).ToString();
    }

    private IEnumerator PlaySoundWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Set decaying volume
        _audioSource.volume = 1f;
        _audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(_rewardParticles.lifetime.constant + 0.3f);
        _audioSource.FadeOut(1f);
    }
}