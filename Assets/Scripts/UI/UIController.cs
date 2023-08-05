using UnityEngine;

public class UIController : MonoBehaviour
{

    public static UIController Instance;

    [SerializeField]
    private GameObject _zombiesUpgradeUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ShowZombiesUpgrade()
    {
        _zombiesUpgradeUI.SetActive(!_zombiesUpgradeUI.activeSelf);
    }

    public void PlayButtonClickSound()
    {
        SoundsManager.Instance.PlaySound(Sounds.ButtonClick);
    }
}