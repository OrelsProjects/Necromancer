using UnityEngine;

public class ZombieSpawnerController : MonoBehaviour
{
    public static ZombieSpawnerController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SpawnZombie(GameObject zombiePrefab, Vector3 position, Quaternion rotation, int amount)
    {
        float spaceBetweenZombies = 2f;
        for (int i = 0; i < amount; i++)
        {
            float angle = i * Mathf.PI * 2f / amount;
            Vector3 spawnPosition = position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spaceBetweenZombies;
            Instantiate(zombiePrefab, spawnPosition, rotation);
        }
    }

    public void SpawnZombie(GameObject zombiePrefab, Vector3 position, int amount)
    {
        SpawnZombie(zombiePrefab, position, Quaternion.identity, amount);
    }
}