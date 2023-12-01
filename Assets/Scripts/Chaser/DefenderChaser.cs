using UnityEngine;


[RequireComponent(typeof(Defender))]
public class DefenderChaser : MonoBehaviour, IChaser<ZombieBehaviour>
{

    private Owner _selfOwner;
    private IChaser<ZombieBehaviour> _chaser;

    public event IChaser<ZombieBehaviour>.TargetChangeDelegate OnTargetChange;

    public ZombieBehaviour Target
    {
        get
        {
            return _chaser.GetTarget();
        }
    }

    void Start()
    {
        Defender defender = GetComponent<Defender>();
        _selfOwner = new() { gameObject = gameObject, priority = OwnerPriority.Default };
        _chaser = new Chaser<ZombieBehaviour>(gameObject, defender.Data.AttackRange, _selfOwner);
    }

    public ZombieBehaviour FindNewTarget() => _chaser.FindNewTarget();

    public void SetTarget(ZombieBehaviour zombie, Owner owner = default) => _chaser.SetTarget(zombie, _selfOwner);

    public TargetDistanceState GetTargetDistanceState() => _chaser.GetTargetDistanceState();

    public ZombieBehaviour GetTarget() => _chaser.GetTarget();

    public void SubscribeToTargetChanges(IChaser<ZombieBehaviour>.TargetChangeDelegate action) =>
        _chaser.SubscribeToTargetChanges(action);
    public void UnsubscribeFromTargetChanges(IChaser<ZombieBehaviour>.TargetChangeDelegate action) =>
        _chaser.UnsubscribeFromTargetChanges(action);

    public void SetOwner(Owner owner) => _chaser.SetOwner(owner);
}






