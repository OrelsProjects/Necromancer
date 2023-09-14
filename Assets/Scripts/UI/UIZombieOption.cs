using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIZombieOption : MonoBehaviour
{
    public GameObject CostContainer;
    public TextMeshProUGUI CostText;
    public Image Image;
    public Button Button;

    [HideInInspector]
    public ZombieType Type;
    [HideInInspector]
    public int PositionInList;

    private void Start()
    {
        Button.onClick.AddListener(() => UIController.Instance.PlayClickSound());
    }

    public void ResetButtonListeners()
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => UIController.Instance.PlayClickSound());
    }

    /// <summary>
    /// Hides the cost ui and centers the zombie image.
    /// </summary>
    public void DisableCost()
    {
        CostContainer.SetActive(false);
        Image.transform.position = new(Image.transform.position.x, 0);
    }
}

