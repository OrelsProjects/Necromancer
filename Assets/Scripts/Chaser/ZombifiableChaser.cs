using UnityEngine;

public class ZombifiableChaser : MonoBehaviour
{
    private IChaser<Zombie> _chaser;

    public Zombie Target
    {
        get
        {
            return _chaser.GetTarget();
        }
    }

    void Awake()
    {
        _chaser = new Chaser<Zombie>(this.gameObject);
    }

    public void SetTarget()
    {
        _chaser.SetTarget();
    }

    public bool IsTargetReached()
    {
        return _chaser.IsTargetReached();
    }
}






