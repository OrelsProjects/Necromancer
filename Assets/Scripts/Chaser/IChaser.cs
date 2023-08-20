using UnityEngine;

public enum TargetDistanceState {
    Reached,
    NotReached,
    NotAvailable,
}

public interface IChaser<T> where T : MonoBehaviour, IChaseable {
    void SetTarget();
    T GetTarget();
    TargetDistanceState GetTargetDistanceState();
}