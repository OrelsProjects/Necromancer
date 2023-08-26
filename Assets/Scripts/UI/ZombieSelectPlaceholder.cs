using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ZombieSelectPlaceholder {
    public bool ShowCostText;
    public bool ShowButton;

    public GameObject Container;
    public Image ZombieImage;
    [ShowIf("ShowCostText")]
    public TMPro.TextMeshProUGUI ZombieCost;
    [ShowIf("ShowButton")]
    public Button Button;

    [HideInInspector]
    public ZombieType SelectedZombie { get; private set; }

    public void SetSelectedZombie(ZombieType type) => SelectedZombie = type;

}