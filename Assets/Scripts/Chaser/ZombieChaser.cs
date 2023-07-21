using UnityEngine;

public class ZombieChaser : MonoBehaviour
{
    private IChaser<Zombifiable> _chaser;
    public Zombifiable Target
    {
        get
        {
            return _chaser.GetTarget();
        }
    }

    void Awake()
    {
        _chaser = new Chaser<Zombifiable>(gameObject);
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