using UnityEngine;

public class ZombieChaser : MonoBehaviour
{

    [SerializeField]
    private float distanceFromTarget = 0.5f;

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
        _chaser = new Chaser<Zombifiable>(gameObject, distanceFromTarget);
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