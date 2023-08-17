using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

    [SerializeField]
    private AudioClip _startClickClip;
    [SerializeField]
    private AudioClip _settingsClickClip;
    [SerializeField]
    private AudioClip _exitClickClip;
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private CanvasRenderer _blackScreen;

    private bool _isStartClicked = false;

    private void Start() {
        _blackScreen.gameObject.SetActive(false);
    }

    public void OnStartClick() {
        if (_isStartClicked) {
            return;
        }

        _audioSource.PlayOneShot(_startClickClip);
        StartCoroutine(PlayStartScene());
        _isStartClicked = true;
    }

    private IEnumerator PlayStartScene() {
        _blackScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _audioSource.FadeOut(1f);
        yield return new WaitForSeconds(0.8f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Round Scene");
    }

    public void OnSettingsClick() {
        _audioSource.PlayOneShot(_settingsClickClip);
        _isStartClicked = false;
    }

    public void OnExitClick() {
        _audioSource.PlayOneShot(_exitClickClip);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        _isStartClicked = false;
    }
}