using UnityEngine;


[RequireComponent(typeof(Defender))]
public class DefenderChaser : MonoBehaviour, IChaser<ZombieBehaviour>
{

    private Owner _selfOwner;
    private IChaser<ZombieBehaviour> _chaser;

    public IChaser<ZombieBehaviour> Chaser
    {
        get
        {
            Defender defender = GetComponent<Defender>();
            _chaser ??= new Chaser<ZombieBehaviour>(gameObject, defender.Data.AttackRange, _selfOwner);
            return _chaser;
        }
    }

    public event IChaser<ZombieBehaviour>.TargetChangeDelegate OnTargetChange;

    public ZombieBehaviour Target
    {
        get
        {
            return Chaser.GetTarget();
        }
    }

    // Has to be OnEnable, because in Start of DefenderBehaviour we subscribe to OnTargetChange and _chaser is not initialized yet.
    void OnEnable()
    {
        _selfOwner = new() { gameObject = gameObject, priority = OwnerPriority.Default };
    }

    public ZombieBehaviour FindNewTarget() => Chaser.FindNewTarget();

    public void SetTarget(ZombieBehaviour zombie, Owner owner = default) => Chaser.SetTarget(zombie, _selfOwner);

    public TargetDistanceState GetTargetDistanceState() => Chaser.GetTargetDistanceState();

    public ZombieBehaviour GetTarget() => Chaser.GetTarget();

    public void SubscribeToTargetChanges(IChaser<ZombieBehaviour>.TargetChangeDelegate action) =>
        Chaser.SubscribeToTargetChanges(action);
    public void UnsubscribeFromTargetChanges(IChaser<ZombieBehaviour>.TargetChangeDelegate action) =>
        Chaser.UnsubscribeFromTargetChanges(action);

    public void SetOwner(Owner owner) => Chaser.SetOwner(owner);
}






