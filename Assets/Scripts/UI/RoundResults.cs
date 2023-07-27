using System.Collections;
using TMPro;
using UnityEngine;
using AssetKits.ParticleImage;
public class RoundResults : MonoBehaviour
{

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

    private float _timeToShowReward = 3f;

    private void Start()
    {
        _title.gameObject.SetActive(true);
        _reward.gameObject.SetActive(false);
        _rewardText.text = RoundManager.Instance.Reward.ToString();
        _currentGoldText.text = InventoryManager.Instance.Currency.ToString();
        StartCoroutine(ShowRewardCoroutine());
    }

    private IEnumerator ShowRewardCoroutine()
    {
        yield return new WaitForSeconds(_timeToShowReward);
        ShowReward();
    }

    public void ShowReward()
    {
        _reward.gameObject.SetActive(true);
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
        float currentCurrency = InventoryManager.Instance.Currency;
        // float currentCurrency = 10;
        float reward = RoundManager.Instance.Reward;
        // float reward = 240;
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