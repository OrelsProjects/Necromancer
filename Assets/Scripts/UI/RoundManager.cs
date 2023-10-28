using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoundState
{
    NotStarted,
    Started,
    ZombiesWon,
    DefendersWon,
    Won,
    Lost,
}

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    [SerializeField]
    private WaveController _waveController;

    [SerializeField]
    private Areas _area;
    [Header("UI")]
    [SerializeField]
    private GameObject _winUI;
    [SerializeField]
    private GameObject _loseUI;
    [SerializeField]
    private GameObject _zombiesSpawnersContainer;
    [SerializeField]
    private ZombieSpawnBehaviour _zombieSpawnerBehaviorPrefab;
    [SerializeField]
    private List<Transform> _defendersPositions;

    [Header("Sound")]
    [SerializeField]
    private AudioClip _winSound;
    [SerializeField]
    private AudioClip _loseSound;
    [SerializeField]
    private AudioClip _zombiesSpawnedSound;
    [SerializeField]
    private AudioSource _audioSource;

    [Header("Playground")]
    [SerializeField]
    private bool _playground = false;

    private readonly List<ZombieBehaviour> _zombies = new();
    private readonly List<Zombifiable> _zombifiables = new();
    private readonly List<Defender> _defenders = new();

    private GameObject _zombiesParent;
    private GameObject _zombifiablesParent;
    private GameObject _defendersParent;
    private AreaData _data;

    private bool _isZombiesSoundPlaying = false;

    private RoundState _state = RoundState.NotStarted;

    public int Reward
    {
        get
        {
            if (_data)
            {
                return _data.RoundData.Reward;
            }
            return 0;
        }
    }

    public RoundState State => _state;

    public bool IsRoundOver => (_state == RoundState.ZombiesWon || _state == RoundState.DefendersWon) && _waveController != null && _waveController.IsWaveInProgress;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() // So it happens before Start of WaveController
    {
        _waveController.Initialize(_area);
    }

    private void Start()
    {
        _data = AreasManager.Instance.GetAreaData(_area);
        StartRound();
    }

    private void Update()
    {
        switch (_state)
        {
            case RoundState.NotStarted:
                HandleNotStartedState();
                break;
            case RoundState.Started:
                HandleStartedState();
                break;
            case RoundState.ZombiesWon:
                HandleZombiesWonState();
                break;
            case RoundState.DefendersWon:
                HandleDefendersWonState();
                break;
        }
    }
    /// <summary>
    /// This function adds 30 spawn points around the map, 30px outside the camera.
    /// </summary>
    private void InitDefendersPositions()
    {
        _defendersPositions = new List<Transform>();
        Transform _defendersPositionsParent = new GameObject("Defenders Positions").transform;
        _defendersPositionsParent.SetParent(transform);
        Vector2 cameraSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        for (int i = 0; i < 30; i++)
        {
            Vector2 randomPosition = new(Random.Range(-cameraSize.x - 1, cameraSize.x + 1), Random.Range(-cameraSize.y - 1, cameraSize.y + 1));
            _defendersPositions.Add(new GameObject("Defender Position").transform);
            _defendersPositions[i].position = randomPosition;
            _defendersPositions[i].SetParent(_defendersPositionsParent);
        }
    }

    private void HandleNotStartedState()
    {
        if (AreThereZombiesAlive())
        {
            _state = RoundState.Started;
            _waveController.StartWaves();
        }
    }

    private void HandleStartedState()
    {

        if (ShouldPlayZombiesSound())
        {
            PlayZombieSpawnSound();
        }
        if (!AreThereZombies())
        {
            PlayBackgroundMusic();
            _state = RoundState.DefendersWon;
            return;
        }
        if (_waveController == null || _waveController.State == WaveState.Done)
        {
            if (!AreThereDefenders())
            {
                ReduceZombiesSoundVolume();
                _state = RoundState.ZombiesWon;
                return;
            }
        }
    }

    private void HandleZombiesWonState()
    {
        SoundsManager.Instance.StopAll();
        AudioSource.PlayClipAtPoint(_winSound, transform.position);
        AreasManager.Instance.ZombifyArea(_data.Area);
        AreasManager.Instance.SaveData();
        ShowWinUI();
        _state = RoundState.Won;
        StartCoroutine(FinishRound());
    }

    private void HandleDefendersWonState()
    {
        SoundsManager.Instance.StopAll();
        AudioSource.PlayClipAtPoint(_loseSound, transform.position);
        ShowLoseUI();
        _state = RoundState.Lost;
        StartCoroutine(FinishRound());
    }

    private bool AreThereZombies()
    {
        bool areThereZombifiedCharacters = _zombifiables.Exists(zombifiable => zombifiable.IsZombified());
        bool areThereZombiesAlive = _zombies.Count > 0;
        bool areThereZombiesToSpawn = ZombieSpawner.Instance.AreThereZombiesToSpawn();

        // Check for zombies in various conditions, including those that might not have reached their Start function yet.
        return areThereZombifiedCharacters
            || areThereZombiesAlive
            || areThereZombiesToSpawn
            || FindAnyObjectByType<ZombieBehaviour>() != null;
    }

    private bool AreThereZombiesAlive()
    {
        return _zombies.Count > 0;
    }

    private bool AreThereDefenders()
    {
        return _defenders.Count > 0;
    }

    public Vector3? GetClosestZombiePosition(Vector3 position)
    {
        float closestDistance = Mathf.Infinity;
        Vector3? closestPosition = null;
        foreach (var zombie in _zombies)
        {
            float distance = Vector3.Distance(position, zombie.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = zombie.transform.position;
            }
        }
        return closestPosition;
    }

    public void StartRound()
    {
        if (!_playground)
        {
            _data = Map.Instance.SelectedArea;
            HideResultUI();
        }
        InitDefendersPositions();
        PlayBackgroundMusic();
        SpawnCivilians();
        _state = RoundState.NotStarted;
    }

    public void SpawnCivilians()
    {
        int counter = 0;
        _data.RoundData.CiviliansPrefabs.Value.ForEach(civ =>
        {
            Vector3 randomPosition = new(Random.Range(-5, 5), Random.Range(-5, 5));
            Zombifiable zombifiableInstance = Instantiate(civ, randomPosition, Quaternion.identity);
            zombifiableInstance.name = zombifiableInstance.name + " " + counter;
            AddZombifiable(zombifiableInstance);
        });
    }

    public void SpawnDefenders()
    {
        var defenders = _data.RoundData.Defenders.Value;
        _data.RoundData.Defenders.Value.ForEach(defender =>
                {
                    Defender defenderInstance = Instantiate(defender, Vector3.zero, Quaternion.identity);
                    AddDefender(defenderInstance);
                });

    }

    public void AddZombie(ZombieBehaviour zombie)
    {
        if (_zombiesParent == null)
        {
            _zombiesParent = new GameObject("Zombies");
        }
        zombie.gameObject.transform.SetParent(_zombiesParent.transform);
        _zombies.Add(zombie);
    }

    public void AddZombifiable(Zombifiable zombifiable)
    {
        if (_zombifiablesParent == null)
        {
            _zombifiablesParent = new GameObject("Zombifiables");
        }
        zombifiable.gameObject.transform.SetParent(_zombifiablesParent.transform);
        _zombifiables.Add(zombifiable);
    }

    public void AddDefender(Defender defender)
    {
        if (_defendersParent == null)
        {
            _defendersParent = new GameObject("Defenders");
        }
        Transform spawnPosition = _defendersParent.transform;

        if (_defendersPositions != null && _defendersPositions.Count > 0)
        {
            spawnPosition = _defendersPositions[Random.Range(0, _defendersPositions.Count)];
        }

        defender.gameObject.transform.SetParent(_defendersParent.transform);
        defender.transform.position = spawnPosition.position;
        _defenders.Add(defender);
    }

    public void RemoveZombie(ZombieBehaviour zombie) => _zombies.Remove(zombie);
    public void RemoveZombifiable(Zombifiable zombifiable) => _zombifiables.Remove(zombifiable);
    public void RemoveDefender(Defender defender) => _defenders.Remove(defender);

    public IEnumerator FinishRound()
    {
        Destroy(gameObject, 2.3f);
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map1");
    }

    private void ShowWinUI()
    {
        _winUI.SetActive(true);
        _loseUI.SetActive(false);
    }

    private void ShowLoseUI()
    {
        _winUI.SetActive(false);
        _loseUI.SetActive(true);
    }

    private void HideResultUI()
    {
        _winUI.SetActive(false);
        _loseUI.SetActive(false);
    }

    private bool ShouldPlayZombiesSound() => AreThereZombiesAlive() && !_isZombiesSoundPlaying;

    private void PlayBackgroundMusic()
    {
        _audioSource.clip = SoundsManager.GetAreaSound(Map.Instance.SelectedArea.Area);
        _audioSource.Play();
    }

    private void PlayZombieSpawnSound()
    {
        _isZombiesSoundPlaying = true;
        _audioSource.clip = SoundsManager.GetBackgroundSound(BackgroundSoundTypes.ZombiesSpawned);
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void ReduceZombiesSoundVolume()
    {
        _audioSource.ChangeSoundOverTime(0.1f, 0.35f);
        _isZombiesSoundPlaying = false;
        StartCoroutine(SetZombiesVolumeBackUp());
    }

    private IEnumerator SetZombiesVolumeBackUp()
    {
        yield return new WaitForSeconds(2.5f);
        _audioSource.ChangeSoundOverTime(0.8f, 1f);
    }
}