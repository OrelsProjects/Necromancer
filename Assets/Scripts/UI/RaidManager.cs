using System.Collections.Generic;
using UnityEngine;

public class RaidManager : MonoBehaviour {
    public static RaidManager Instance;


    private List<ZombieType> _raidZombies;

    public List<ZombieType> RaidZombies {
        get {
            return _raidZombies;
        }
        set {
            _raidZombies = value;
        }
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void StartRaid(Areas area, List<ZombieType> zombies, int cost) {
        RaidZombies = zombies;
        InventoryManager.Instance.UseCurrency(cost);
        Map.Instance.LoadArea(area);
    }
}
