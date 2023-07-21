using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    private List<Zombie> _zombies = new();
    private List<Zombifiable> _zombifiables = new();
    private List<Defender> _defenders = new();

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

    public void AddZombie(Zombie zombie)
    {
        _zombies.Add(zombie);
    }

    public void RemoveZombie(Zombie zombie)
    {
        _zombies.Remove(zombie);
    }

    public void AddZombifiable(Zombifiable zombifiable)
    {
        _zombifiables.Add(zombifiable);
    }

    public void RemoveZombifiable(Zombifiable zombifiable)
    {
        _zombifiables.Remove(zombifiable);
    }

    public void AddDefender(Defender defender)
    {
        _defenders.Add(defender);
    }

    public void RemoveDefender(Defender defender)
    {
        _defenders.Remove(defender);
    }
}