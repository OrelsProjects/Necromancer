using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoundState
{
    NotStarted,
    Started,
    ZombiesWon,
    DefendersWon,
    Done
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
    private GameObject _abandonRaidObject;
    [SerializeField]
    private GameObject _speedFastImage;

    [SerializeField]
    private List<Transform> _defendersPositions;

    [Header("Sound")]
    [SerializeField]
    private AudioClip _zombiesSpawnedSound;
    [SerializeField]
    private AudioSource _audioSource;

    [Header("Playground")]
    [SerializeField]
    private bool _playground = false;

    private readonly float _timeToCloseRound = 2f;
    private readonly List<ZombieBehaviour> _zombies = new();
    private readonly List<Zombifiable> _zombifiables = new();
    private readonly List<Defender> _defenders = new();

    private GameObject _zombiesParent;
    private GameObject _zombifiablesParent;
    private GameObject _defendersParent;
    private AreaData _data;


    private float _currentGameSpeed = 1;
    private readonly float _minGameSpeed = 1;
    private readonly float _maxGameSpeed = 2f;
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
            Vector2 randomPosition = new(Random.Range(-cameraSize.x + 5, cameraSize.x - 5), Random.Range(-cameraSize.y + 6, cameraSize.y - 6));
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
            if (ShouldPlayZombiesSound())
            {
                ChangeSoundFromAreaToZombies();
            }
        }
    }

    private void HandleStartedState()
    {
        if (DidDefendersWin())
        {
            ReduceZombiesSoundVolume();
            _state = RoundState.DefendersWon;
            return;
        }
        if (DidZombiesWin())
        {
            ReduceZombiesSoundVolume();
            _state = RoundState.ZombiesWon;
            return;
        }
    }

    private void HandleZombiesWonState()
    {
        SoundsManager.Instance.StopAll();
        AudioSourceHelper.PlayClipAtPoint(UISoundTypes.RoundWin, 0.25f);
        AreasManager.Instance.ZombifyArea(_data.Area);
        AreasManager.Instance.SaveData();
        ShowWinUI();
        FinishRound();
    }

    private void HandleDefendersWonState()
    {
        SoundsManager.Instance.StopAll();
        AudioSourceHelper.PlayClipAtPoint(UISoundTypes.RoundLose, 0.25f);
        ShowLoseUI();
        FinishRound();
    }

    #region WIN CONDITIONS
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

    private bool AreThereDefenders => _defenders.Count > 0;

    private bool AreThereCivilians =>
         _zombifiables.Exists(zombifiable => zombifiable.GetComponent<Civilian>() != null);

    private bool AreWavesFinished => _waveController == null || _waveController.State == WaveState.Done;


    private bool DidZombiesWin() => (!AreThereDefenders && AreWavesFinished) || !AreThereCivilians;
    private bool DidDefendersWin() => !AreThereZombies();
    #endregion

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

    public bool IsRoundStarted => _state != RoundState.NotStarted;

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

    public void SpeedUpGame()
    {
        if (_state == RoundState.ZombiesWon || _state == RoundState.DefendersWon)
        {
            return;
        }
        _currentGameSpeed = _currentGameSpeed == _minGameSpeed ? _maxGameSpeed : _minGameSpeed;
        _speedFastImage.SetActive(_currentGameSpeed == _minGameSpeed);
        Time.timeScale = _currentGameSpeed;
    }

    public void ShowAbandonRaid(bool shouldShow)
    {
        _abandonRaidObject.SetActive(shouldShow);
        Time.timeScale = shouldShow ? _minGameSpeed : _currentGameSpeed;
    }


    public void FinishRound(bool immediate = false)
    {
        RoundState previousState = _state;
        Time.timeScale = _minGameSpeed;
        _state = RoundState.Done;
        _waveController.StopWaves();
        if (previousState == RoundState.ZombiesWon)
        {
            InventoryManager.Instance.AddCurrency(Reward);
        }
        if (immediate)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Map1");
        }
        else
        {
            StartCoroutine(FinishRoundCore());
        }
    }

    private IEnumerator FinishRoundCore()
    {
        Destroy(gameObject, 2.3f);
        yield return new WaitForSeconds(_timeToCloseRound);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map1");
        Time.timeScale = 1f;
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
        _audioSource.FadeIn(0.5f, 1f);
    }

    // Assuming the current clip is Area's clip.
    private void ChangeSoundFromAreaToZombies()
    {
        _audioSource.FadeOut(1f, PlayZombieSpawnSound);
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