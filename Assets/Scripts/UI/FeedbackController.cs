using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackController : MonoBehaviour
{
    [SerializeField]
    private GameObject _sendText;
    [SerializeField]
    private GameObject _sendLoading;
    [SerializeField]
    private GameObject _feedbackPanel;
    [SerializeField]
    private TextMeshProUGUI _feedbackText;
    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private GameObject _thankYou;

    [Header("Rating")]
    [SerializeField]
    private List<Image> _feedbackStars;
    [SerializeField]
    private ParticleSystem _ratingParticles;

    private int _stars = 0;

    public void SetStars(int stars)
    {
        _stars = stars;
        for (int i = 0; i < stars; i++)
        {
            Image image = _feedbackStars[i];
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            if (i == stars - 1)
            {
                _ratingParticles.transform.position = image.transform.position;
                _ratingParticles.gameObject.SetActive(false);
                _ratingParticles.gameObject.SetActive(true);
                _ratingParticles.Play();
                SoundsManager.PlayAtPointUISound(UISoundTypes.Rating);
            }
        }
        for (int i = stars; i < 5; i++)
        {
            Image image = _feedbackStars[i];
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
        }
    }

    public void ShowFeedback()
    {
        _feedbackPanel.SetActive(true);
    }

    public void HideFeedback()
    {
        _feedbackPanel.SetActive(false);
    }

    public void SendFeedback()
    {
        LoadFeedback(true);
        SendFeedbackCoroutine();
    }

    private void LoadFeedback(bool isLoading)
    {
        _sendLoading.SetActive(isLoading);
        _sendText.SetActive(!isLoading);
    }

    private async void SendFeedbackCoroutine()
    {
        try
        {
            await SendFeedback(_nameText.text, _feedbackText.text, _stars,
                onFeedbackFinished: () =>
                {
                    LoadFeedback(false);
                    StartCoroutine(ShowThankYou());
                }
            );
        }
        catch (Exception e)
        {
            Debug.Log(e);
            LoadFeedback(false);
        }
    }

    private IEnumerator ShowThankYou()
    {
        _thankYou.SetActive(true);
        yield return new WaitForSeconds(3f);
        _thankYou.SetActive(false);
        UIController.Instance.HideCompletedScreen();
    }

    private async Task SendFeedback(string feedback, string name = "Unknown",
      int stars = 0,
      Action onFeedbackSent = null, Action onFeedbackFailed = null,
      Action onFeedbackFinished = null
  )
    {
        string url = "https://addfeedback-iuucqmv7fa-uc.a.run.app";

        var httpClient = new HttpClient();
        string requestBody = "{\"feedback\": \"" + feedback + "\", \"name\": \"" + name + "\", \"rating\": " + stars + "}";
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                Debug.Log("Feedback sent successfully");
                onFeedbackSent?.Invoke();
            }
            else
            {
                Debug.LogError("Feedback send failed: " + response.ReasonPhrase);
                onFeedbackFailed?.Invoke();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Feedback send failed: " + e.Message);
        }
        finally
        {
            onFeedbackFinished?.Invoke();
        }
    }
}