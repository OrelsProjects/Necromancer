using UnityEngine;

[RequireComponent(typeof(Defender))]
public class DefenderChaser : MonoBehaviour, IChaser<Zombie> {

    [SerializeField]
    private float distanceFromTarget = 0.5f;

    private IChaser<Zombie> _chaser;

    public event IChaser<Zombie>.TragetChangeDelegate OnTargetChange;

    public Zombie Target {
        get {
            return _chaser.GetTarget();
        }
    }

    void Awake() {
        Defender defender = GetComponent<Defender>();
        _chaser = new Chaser<Zombie>(gameObject, defender.Data.AttackRange);
    }

    public Zombie FindNewTarget() => _chaser.FindNewTarget();

    public void SetTarget(Zombie zombie) => _chaser.SetTarget(zombie);

    public TargetDistanceState GetTargetDistanceState() => _chaser.GetTargetDistanceState();

    public Zombie GetTarget() => _chaser.GetTarget();

    public void SubscribeToTargetChanges(IChaser<Zombie>.TragetChangeDelegate action) =>
        _chaser.SubscribeToTargetChanges(action);
    public void UnsubscribeFromTargetChanges(IChaser<Zombie>.TragetChangeDelegate action) =>
        _chaser.UnsubscribeFromTargetChanges(action);
}






