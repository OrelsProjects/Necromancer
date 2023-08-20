using UnityEngine;

public enum TargetDistanceState {
    Reached,
    NotReached,
    NotAvailable,
}

public interface IChaser<T> where T : MonoBehaviour, IChaseable {
    public delegate void TragetChangeDelegate(T target);
    public event TragetChangeDelegate OnTargetChange;

    T FindNewTarget();
    void SetTarget(T target);
    T GetTarget();
    TargetDistanceState GetTargetDistanceState();
    void SubscribeToTargetChanges(TragetChangeDelegate action);
    void UnsubscribeFromTargetChanges(TragetChangeDelegate action);
}