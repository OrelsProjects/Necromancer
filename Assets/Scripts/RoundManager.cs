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

    private List<Zombie> _zombies = new();
    private List<Zombifiable> _zombifiables = new();
    private List<Defender> _defenders = new();

    private GameObject _zombiesParent;
    private GameObject _zombifiablesParent;
    private GameObject _defendersParent;

    private RoundState _state = RoundState.NotStarted;

    private bool _sendDefendersToBattle = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _zombiesParent = new GameObject("Zombies");
            _zombifiablesParent = new GameObject("Zombifiables");
            _defendersParent = new GameObject("Defenders");
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
        Debug.Log("Zombies won");
        _state = RoundState.Done;
    }

    private void HandleDefendersWonState()
    {
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

    public void AddZombie(Zombie zombie)
    {
        zombie.gameObject.transform.SetParent(_zombiesParent.transform);
        _zombies.Add(zombie);
        _sendDefendersToBattle = true;
    }

    public void RemoveZombie(Zombie zombie)
    {
        _zombies.Remove(zombie);
    }

    public void AddZombifiable(Zombifiable zombifiable)
    {
        zombifiable.gameObject.transform.SetParent(_zombifiablesParent.transform);
        _zombifiables.Add(zombifiable);
    }

    public void RemoveZombifiable(Zombifiable zombifiable)
    {
        _zombifiables.Remove(zombifiable);
    }

    public void AddDefender(Defender defender)
    {
        defender.gameObject.transform.SetParent(_defendersParent.transform);
        _defenders.Add(defender);
        Zombifiable zombifiableComponent = defender.GetComponent<Zombifiable>();
        if (zombifiableComponent != null)
        {
            _zombifiables.Add(zombifiableComponent);
        }
    }

    public void RemoveDefender(Defender defender)
    {
        _defenders.Remove(defender);
        Zombifiable zombifiableComponent = defender.GetComponent<Zombifiable>();
        if (zombifiableComponent != null)
        {
            _zombifiables.Remove(zombifiableComponent);
        }
    }
}