using UnityEngine;

public class ZombieChaser : MonoBehaviour, IChaser<Zombifiable> {
    [SerializeField]
    private float distanceFromTarget = 0.5f;

    private IChaser<Zombifiable> _chaser;

    public event IChaser<Zombifiable>.TragetChangeDelegate OnTargetChange;

    public Zombifiable Target => _chaser.GetTarget();

    void Awake() {
        _chaser = new Chaser<Zombifiable>(gameObject, distanceFromTarget);
    }

    public Zombifiable FindNewTarget() => _chaser.FindNewTarget();

    public void SetTarget(Zombifiable target) => _chaser.SetTarget(target);

    public TargetDistanceState GetTargetDistanceState() => _chaser.GetTargetDistanceState();

    public Zombifiable GetTarget() => _chaser.GetTarget();

    public void SubscribeToTargetChanges(IChaser<Zombifiable>.TragetChangeDelegate action) =>
        _chaser.SubscribeToTargetChanges(action);

    public void UnsubscribeFromTargetChanges(IChaser<Zombifiable>.TragetChangeDelegate action) =>
        _chaser.UnsubscribeFromTargetChanges(action);
}