using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum PodState
{
    Idle,
    Spawning,
    Destroyed
}

public class PodController : MonoBehaviour
{
    [SerializeField]
    private PodData _podData;
    [SerializeField]
    private Transform _spawnPoint;
    [SerializeField]
    private Image _loadImage;

    private PodState _state = PodState.Idle;
    private int _currentLevel = 1;
    private int _currentHealth;

    private PodLevel Level { get { return _podData.GetLevel(_currentLevel); } }

    void Start()
    {
        _currentHealth = Level.Health;
    }

    void Update()
    {
        switch (_state)
        {
            case PodState.Idle:
                if (Level.SpawnRate.count > 0)
                {
                    StartCoroutine(SpawnZombies());
                }
                break;
            case PodState.Destroyed:
                Destroy(gameObject);
                break;
        }
    }

    private IEnumerator SpawnZombies()
    {
        _state = PodState.Spawning;
        float currentTime = 0;
        Zombie zombiePrefab = CharactersManager.Instance.GetZombiePrefab(Level.ZombieType);
        while (currentTime < Level.SpawnRate.time)
        {
            currentTime += Time.deltaTime;
            _loadImage.fillAmount = currentTime / Level.SpawnRate.time;
            yield return null;
        }
        for (var i = 0; i < Level.SpawnRate.count; i++)
        {
            Instantiate(zombiePrefab, _spawnPoint.position, Quaternion.identity);
        }
        _state = PodState.Idle;
    }

    public void TakeDamage(int damage)
    {
        int newHealth = _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(newHealth, 0, Level.Health);
        if (newHealth == 0)
        {
            _state = PodState.Destroyed;
        }
    }
}

