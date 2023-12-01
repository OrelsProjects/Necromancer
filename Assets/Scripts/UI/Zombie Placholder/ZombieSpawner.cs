using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ZombieSpawner : MonoBehaviour
{

    public static ZombieSpawner Instance;

    [SerializeField]
    private ZombieSpawnBehaviour _zombieSpawnerBehaviorPrefab;
    [SerializeField]
    private GameObject _zombieSpawnersContainer;

    private readonly Dictionary<ZombieType, ZombieSpawnBehaviour> _zombieTypeToZombieSpawnBehaviour = new();
    private ZombieSpawnBehaviour _selectedZombieSpawnBehaviour;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        InitSpawnBar();
    }

    void Update()
    {
        SpawnSelectedZombie();
    }

    public void SpawnSelectedZombie()
    {
        if (Input.touchCount > 0 && _selectedZombieSpawnBehaviour != null)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = touch.position;
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane));
                _selectedZombieSpawnBehaviour.SpawnZombies(worldPosition);
                SelectZombie(null);
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && _selectedZombieSpawnBehaviour != null)
        {
            Vector2 touchPosition = Input.mousePosition;
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane));
            _selectedZombieSpawnBehaviour.SpawnZombies(worldPosition);
            _selectedZombieSpawnBehaviour.GetComponent<Button>().interactable = false;
            SelectZombie(null);
        }
#endif
    }

    private void InitSpawnBar()
    {
        RaidManager.Instance.RaidZombies.ForEach(zombie =>
        {
            ZombieSpawnBehaviour zombieSpawner = InitSpawningBehaviour(zombie);
            _zombieTypeToZombieSpawnBehaviour.TryAdd(zombie, zombieSpawner);
        });
    }

    private ZombieSpawnBehaviour InitSpawningBehaviour(ZombieType zombie)
    {
        ZombieSpawnBehaviour zombieSpawner = Instantiate(_zombieSpawnerBehaviorPrefab);
        zombieSpawner.SetZombieType(zombie);
        Button button = zombieSpawner.gameObject.GetComponent<Button>();
        button.onClick.AddListener(() => SelectZombie(zombieSpawner));
        zombieSpawner.transform.SetParent(_zombieSpawnersContainer.transform);
        zombieSpawner.transform.localScale = new(0.8f, 0.8f);

        return zombieSpawner;
    }

    private void SelectZombie(ZombieSpawnBehaviour zombieSpawner)
    {
        if (_selectedZombieSpawnBehaviour != null)
        {
            _selectedZombieSpawnBehaviour.Select(false);
        }
        if (zombieSpawner != null)
        {
            _selectedZombieSpawnBehaviour = zombieSpawner;
            _selectedZombieSpawnBehaviour.Select(true);
        }
    }

    public void SelectZombie(ZombieType zombieType)
    {
        SelectZombie(_zombieTypeToZombieSpawnBehaviour[zombieType]);
    }

    public bool AreThereZombiesToSpawn()
    {
        foreach (var zombie in _zombieTypeToZombieSpawnBehaviour)
        {
            if (zombie.Value.IsAvailable)
            {
                return true;
            }
        }
        return false;
    }
}