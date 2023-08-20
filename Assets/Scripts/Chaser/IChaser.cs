using UnityEngine;

public enum TargetDistanceState {
    Reached,
    NotReached,
    NotAvailable,
}

public interface IChaser<T> where T : MonoBehaviour, IChaseable {
    void FindNewTarget();
    void SetTarget(T target);
    T GetTarget();
    TargetDistanceState GetTargetDistanceState();
}