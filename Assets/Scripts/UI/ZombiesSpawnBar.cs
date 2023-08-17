using System.Collections.Generic;
using UnityEngine;

public class ZombiesSpawnBar : MonoBehaviour {

    public static ZombiesSpawnBar Instance;

    [SerializeField]
    private List<ZombiePlaceholder> _zombiePlaceholders = new();

    public bool AreThereZombiesToSpawn() {
        try {
            return _zombiePlaceholders.Find(ZombiePlaceholder => ZombiePlaceholder.IsAvailable) != null;
        } catch {
            return false;
        }
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void SetUpZombies(List<ZombiePlaceholder> zombiePlaceholders) {
        _zombiePlaceholders = zombiePlaceholders;
    }
}