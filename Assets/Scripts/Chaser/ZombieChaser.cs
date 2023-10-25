using UnityEngine;

public class ZombieChaser : MonoBehaviour, IChaser<Zombifiable>
{
    [SerializeField]
    private float distanceFromTarget = 2f;

    private Owner _selfOwner;

    private IChaser<Zombifiable> _chaser;

    public event IChaser<Zombifiable>.TargetChangeDelegate OnTargetChange;

    public Zombifiable Target => _chaser.GetTarget();

    void Awake()
    {
        _selfOwner = new() { gameObject = null, priority = OwnerPriority.Default };
        _chaser = new Chaser<Zombifiable>(gameObject, distanceFromTarget, _selfOwner);
    }

    public Zombifiable FindNewTarget() => _chaser.FindNewTarget();

    public void SetTarget(Zombifiable zombifiable, Owner owner) => _chaser.SetTarget(zombifiable, owner);

    public TargetDistanceState GetTargetDistanceState() => _chaser.GetTargetDistanceState();

    public Zombifiable GetTarget() => _chaser.GetTarget();

    public void SubscribeToTargetChanges(IChaser<Zombifiable>.TargetChangeDelegate action) =>
        _chaser.SubscribeToTargetChanges(action);

    public void UnsubscribeFromTargetChanges(IChaser<Zombifiable>.TargetChangeDelegate action) =>
        _chaser.UnsubscribeFromTargetChanges(action);

    public void SetOwner(Owner owner) => _chaser.SetOwner(owner);
}