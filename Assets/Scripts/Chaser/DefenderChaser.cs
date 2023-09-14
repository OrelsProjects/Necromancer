using UnityEngine;


[RequireComponent(typeof(Defender))]
public class DefenderChaser : MonoBehaviour, IChaser<Zombie>
{

    private Owner _selfOwner;
    private IChaser<Zombie> _chaser;

    public event IChaser<Zombie>.TargetChangeDelegate OnTargetChange;

    public Zombie Target
    {
        get
        {
            return _chaser.GetTarget();
        }
    }

    void Awake()
    {
        Defender defender = GetComponent<Defender>();
        _selfOwner = new() { gameObject = gameObject, priority = OwnerPriority.Default };
        _chaser = new Chaser<Zombie>(gameObject, defender.Data.AttackRange, _selfOwner);
    }

    public Zombie FindNewTarget() => _chaser.FindNewTarget();

    public void SetTarget(Zombie zombie, Owner owner = default) => _chaser.SetTarget(zombie, _selfOwner);

    public TargetDistanceState GetTargetDistanceState() => _chaser.GetTargetDistanceState();

    public Zombie GetTarget() => _chaser.GetTarget();

    public void SubscribeToTargetChanges(IChaser<Zombie>.TargetChangeDelegate action) =>
        _chaser.SubscribeToTargetChanges(action);
    public void UnsubscribeFromTargetChanges(IChaser<Zombie>.TargetChangeDelegate action) =>
        _chaser.UnsubscribeFromTargetChanges(action);

    public void SetOwner(Owner owner) => _chaser.SetOwner(owner);
}






