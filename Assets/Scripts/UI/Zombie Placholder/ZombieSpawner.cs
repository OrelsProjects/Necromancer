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

    private readonly Dictionary<ZombieType, ZombieSpawnBehaviour> _zombiesList = new();
    private ZombieSpawnBehaviour _selectedZombie;

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
        if (Input.touchCount > 0 && _selectedZombie != null)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = touch.position;
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane));
                _selectedZombie.SpawnZombies(worldPosition);
                SelectZombie(null);
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && _selectedZombie != null)
        {
            Vector2 touchPosition = Input.mousePosition;
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane));
            _selectedZombie.SpawnZombies(worldPosition);
            _selectedZombie.GetComponent<Button>().interactable = false;
            SelectZombie(null);
        }
#endif
    }


    private void InitSpawnBar()
    {
        RaidManager.Instance.RaidZombies.ForEach(zombie =>
        {
            ZombieSpawnBehaviour zombieSpawner = InitSpawningBehaviour(zombie);
            _zombiesList.Add(zombie, zombieSpawner);
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
        if (_selectedZombie != null)
        {
            _selectedZombie.Select(false);
        }
        if (zombieSpawner != null)
        {
            _selectedZombie = zombieSpawner;
            _selectedZombie.Select(true);
        }
    }

    public bool AreThereZombiesToSpawn()
    {
        foreach (var zombie in _zombiesList)
        {
            if (zombie.Value.IsAvailable)
            {
                return true;
            }
        }
        return false;
    }
}

