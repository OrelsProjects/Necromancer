using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaidZombieOption : MonoBehaviour
{
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

}

