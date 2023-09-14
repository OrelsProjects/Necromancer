using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Areas _area;
    [Header("UI")]
    [SerializeField]
    private GameObject _roundResultsUI;
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

    private readonly List<Zombie> _zombies = new();
    private readonly List<Zombifiable> _zombifiables = new();
    private readonly List<Defender> _defenders = new();

    private GameObject _zombiesParent;
    private GameObject _zombifiablesParent;
    private GameObject _defendersParent;
    private AreaData _data;

    private bool _areDefendersIdle = true;
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
                _state = AreThereZombiesAlive() ? RoundState.Started : RoundState.NotStarted;
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

        if (AreThereZombiesAlive() && _areDefendersIdle && (_state != RoundState.Won || _state != RoundState.Lost))
        {
            PlayZombieSpawnSound();
            SendDefenders();
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
        if (!AreThereDefenders())
        {
            ReduceZombiesSoundVolume();
            _state = RoundState.ZombiesWon;
            return;
        }
    }

    private void HandleZombiesWonState()
    {
        SoundsManager.Instance.StopAll();
        AudioSource.PlayClipAtPoint(_winSound, transform.position);
        AreasManager.Instance.ZombifyArea(_data.Area);
        AreasManager.Instance.SaveData();
        _roundResultsUI.SetActive(true);
        _state = RoundState.Won;
    }

    private void HandleDefendersWonState()
    {
        SoundsManager.Instance.StopAll();
        AudioSource.PlayClipAtPoint(_loseSound, transform.position);
        _roundResultsUI.SetActive(true);
        _state = RoundState.Lost;
    }

    private bool AreThereZombies()
    {
        bool areThereZombifiedCivilians = _zombifiables.Find(zombifiable => zombifiable.IsZombified()) != null;
        bool areThereZombies = _zombies.Count > 0;
        bool areThereZombiesToSpawn = ZombieSpawner.Instance.AreThereZombiesToSpawn();
        return areThereZombifiedCivilians || areThereZombies || areThereZombiesToSpawn;
    }

    private bool AreThereZombiesAlive()
    {
        return _zombies.Count > 0;
    }

    private bool AreThereDefenders()
    {
        return _defenders.Count > 0;
    }

    private void SendDefenders()
    {
        foreach (var defender in _defenders)
        {
            defender.StartBattle();
        }
        _areDefendersIdle = false;
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
            _roundResultsUI.SetActive(false);
        }
        PlayBackgroundMusic();
        _data.RoundData.CiviliansPrefabs.Value.ForEach(civ =>
        {
            Vector3 randomPosition = new(Random.Range(-5, 5), Random.Range(-5, 5));
            Zombifiable zombifiableInstance = Instantiate(civ, randomPosition, Quaternion.identity);
            AddZombifiable(zombifiableInstance);
        });
        _data.RoundData.Defenders.Value.ForEach(defender =>
        {
            Defender defenderInstance = Instantiate(defender, Vector3.zero, Quaternion.identity);
            AddDefender(defenderInstance);
        });
        _state = RoundState.NotStarted;
    }

    public void AddZombie(Zombie zombie)
    {
        if (_zombiesParent == null)
        {
            _zombiesParent = new GameObject("Zombies");
        }
        zombie.gameObject.transform.SetParent(_zombiesParent.transform);
        _zombies.Add(zombie);
        _areDefendersIdle = true;
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

    public void RemoveZombie(Zombie zombie) => _zombies.Remove(zombie);
    public void RemoveZombifiable(Zombifiable zombifiable) => _zombifiables.Remove(zombifiable);
    public void RemoveDefender(Defender defender) => _defenders.Remove(defender);

    public void FinishRound()
    {
        Destroy(gameObject);
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