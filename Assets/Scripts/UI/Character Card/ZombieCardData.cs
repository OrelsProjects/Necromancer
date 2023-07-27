using UnityEngine;

[CreateAssetMenu(fileName = "ZombieCardData", menuName = "Necromancer/Cards/Zombie", order = 1)]
public class ZombieCardData : ScriptableObject
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private int _health;
    [SerializeField]
    private int _speed;
    [SerializeField]
    private int _power;
    [SerializeField]
    private int _cost;
    
    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public int Health
    {
        get => _health;
        set => _health = value;
    }

    public int Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public int Power
    {
        get => _power;
        set => _power = value;
    }

    public int Cost
    {
        get => _cost;
        set => _cost = value;
    }
}