using UnityEngine;

[CreateAssetMenu(fileName = "ZombieHolder", menuName = "Necromancer/Zombie Holder", order = 2)]
public class ZombieHolder : ScriptableObject {
    [SerializeField]
    private int _amount;
    [SerializeField]
    private GameObject _zombiePrefab;
    [SerializeField]
    private Sprite _zombieSprite;

    public int Amount {
        get => _amount;
        set => _amount = value;
    }

    public GameObject ZombiePrefab {
        get => _zombiePrefab;
        set => _zombiePrefab = value;
    }

    public Sprite ZombieSprite {
        get => _zombieSprite;
        set => _zombieSprite = value;
    }
}