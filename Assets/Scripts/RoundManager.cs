using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    private List<Zombie> _zombies = new();
    private List<Zombifiable> _zombifiables = new();
    private List<Defender> _defenders = new();

    private GameObject _zombiesParent;
    private GameObject _zombifiablesParent;
    private GameObject _defendersParent;

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

    private void Update()
    {
        if (_zombies.Count > 0 && _sendDefendersToBattle)
        {
            SendDefenders();
        }
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
    }

    public void RemoveDefender(Defender defender)
    {
        _defenders.Remove(defender);
    }
}