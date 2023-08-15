using UnityEngine;

[RequireComponent(typeof(Defender))]
public class DefenderChaser : MonoBehaviour {

    [SerializeField]
    private float distanceFromTarget = 0.5f;

    private IChaser<Zombie> _chaser;

    public Zombie Target {
        get {
            return _chaser.GetTarget();
        }
    }

    void Awake() {
        Defender defender = GetComponent<Defender>();
        _chaser = new Chaser<Zombie>(gameObject, defender.Data.AttackRange);
    }

    public void SetTarget() {
        _chaser.SetTarget();
    }

    public bool IsTargetReached() {
        return _chaser.IsTargetReached();
    }
}






