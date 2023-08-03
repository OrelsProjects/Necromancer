using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeZombieData", menuName = "Necromancer/Upgrade Zombie", order = 1)]
public class UpgradeZombieData : ScriptableObject
{
    [SerializeField]
    private Sprite _currentZombieSprite;
    [SerializeField]
    private Sprite _newZombieSprite;
    [SerializeField]
    private int _cost;


    public Sprite CurrentZombieSprite
    {
        get => _currentZombieSprite;
        set => _currentZombieSprite = value;
    }

    public Sprite NewZombieSprite
    {
        get => _newZombieSprite;
        set => _newZombieSprite = value;
    }

    public int Cost
    {
        get => _cost;
        set => _cost = value;
    }
}