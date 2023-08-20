using UnityEngine;

public class ZombieChaser : MonoBehaviour {
    [SerializeField]
    private float distanceFromTarget = 0.5f;

    private IChaser<Zombifiable> _chaser;
    public Zombifiable Target {
        get {
            return _chaser.GetTarget();
        }
    }

    void Awake() {
        _chaser = new Chaser<Zombifiable>(gameObject, distanceFromTarget);
    }

    public void FindNewTarget() => _chaser.FindNewTarget();

    public void SetTarget(Zombifiable target) {
        _chaser.SetTarget(target);
    }

    public TargetDistanceState GetTargetDistanceState() {
        return _chaser.GetTargetDistanceState();
    }
}