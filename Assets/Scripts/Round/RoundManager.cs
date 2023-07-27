using System.Collections.Generic;
using UnityEngine;

public enum RoundState
{
    NotStarted,
    Started,
    ZombiesWon,
    DefendersWon,
    Done,
}

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    [SerializeField]
    private RoundData _roundData;
    [Header("UI")]
    [SerializeField]
    private GameObject _roundResultsZombiesWon;

    [Header("Sound")]
    [SerializeField]
    private AudioClip _winSound;
    [SerializeField]
    private AudioClip _loseSound;

    private List<Zombie> _zombies = new();
    private List<Zombifiable> _zombifiables = new();
    private List<Defender> _defenders = new();

    private GameObject _zombiesParent;
    private GameObject _zombifiablesParent;
    private GameObject _defendersParent;

    private AudioSource _audioSource;

    private bool _sendDefendersToBattle = false;

    private RoundState _state = RoundState.NotStarted;

    public float Reward => _roundData.Reward;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _zombiesParent = new GameObject("Zombies");
            _zombifiablesParent = new GameObject("Zombifiables");
            _defendersParent = new GameObject("Defenders");
            _audioSource = GetComponent<AudioSource>();
            StartRound();
        }
        else
        {
            Destroy(gameObject);
        }
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

    private void Update()
    {
        switch (_state)
        {
            case RoundState.NotStarted:
                _state = AreThereZombies() ? RoundState.Started : RoundState.NotStarted;
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

        if (_zombies.Count > 0 && _sendDefendersToBattle)
        {
            SendDefenders();
        }
    }

    private void HandleStartedState()
    {
        if (!AreThereZombies())
        {
            _state = RoundState.DefendersWon;
        }
        else if (!AreThereDefenders())
        {
            _state = RoundState.ZombiesWon;
        }
    }

    private void HandleZombiesWonState()
    {
        _audioSource.PlayOneShot(_winSound);
        _roundResultsZombiesWon.SetActive(true);
        _state = RoundState.Done;
    }

    private void HandleDefendersWonState()
    {
        _audioSource.PlayOneShot(_loseSound);
        Debug.Log("Defenders won");
        _state = RoundState.Done;
    }

    private bool AreThereZombies()
    {
        bool areThereZombifiedCivilians = _zombifiables.Find(zombifiable => zombifiable.IsZombified()) != null;
        bool areThereZombies = _zombies.Count > 0;
        return areThereZombifiedCivilians || areThereZombies;
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
        _sendDefendersToBattle = false;
    }

    public void StartRound()
    {
        for (int i = 0; i < _roundData.CiviliansCount; i++)
        {
            int randomCivilianIndex = Random.Range(0, _roundData.CiviliansPrefabs.Count);
            Vector3 randomPosition = new(Random.Range(-5, 5), Random.Range(-5, 5));
            Zombifiable civilianPrefab = _roundData.CiviliansPrefabs[randomCivilianIndex];
            Zombifiable zombifiableInstance = Instantiate(civilianPrefab, randomPosition, Quaternion.identity);
            zombifiableInstance.transform.SetParent(_zombifiablesParent.transform);
        }

        _roundData.Defenders.ForEach(defender =>
        {
            Defender defenderInstance = Instantiate(defender, Vector3.zero, Quaternion.identity);
            defenderInstance.transform.SetParent(_defendersParent.transform);
        });
    }

    public void AddZombie(Zombie zombie)
    {
        zombie.gameObject.transform.SetParent(_zombiesParent.transform);
        _zombies.Add(zombie);
        _sendDefendersToBattle = true;
    }

    public void AddZombifiable(Zombifiable zombifiable)
    {
        zombifiable.gameObject.transform.SetParent(_zombifiablesParent.transform);
        _zombifiables.Add(zombifiable);
    }

    public void AddDefender(Defender defender)
    {
        defender.gameObject.transform.SetParent(_defendersParent.transform);
        _defenders.Add(defender);
    }

    public void RemoveZombie(Zombie zombie)
    {
        _zombies.Remove(zombie);
        if (_zombies.Count == 0)
        {
            Destroy(_zombiesParent);
        }
    }

    public void RemoveZombifiable(Zombifiable zombifiable)
    {
        _zombifiables.Remove(zombifiable);
        if (_zombifiables.Count == 0)
        {
            Destroy(_zombifiablesParent);
        }
    }

    public void RemoveDefender(Defender defender)
    {
        _defenders.Remove(defender);
        if (_defenders.Count == 0)
        {
            Destroy(_defendersParent);
        }
    }
}