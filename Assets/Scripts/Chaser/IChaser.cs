using UnityEngine;

public enum TargetDistanceState
{
    Reached,
    NotReached,
    NotAvailable,
}


public enum OwnerPriority
{
    Default,
    High,
}

public struct Owner
{
    public GameObject gameObject;
    public OwnerPriority priority;
}

public interface IChaser<T> where T : MonoBehaviour, IChaseable
{
    public delegate void TargetChangeDelegate(T target);
    public event TargetChangeDelegate OnTargetChange;

    T FindNewTarget();
    T GetTarget();
    TargetDistanceState GetTargetDistanceState();

    /// <summary>
    /// Only owners can set new targets.
    /// </summary>
    void SetOwner(Owner owner);
    void SetTarget(T target, Owner owner);
    void SubscribeToTargetChanges(TargetChangeDelegate action);
    void UnsubscribeFromTargetChanges(TargetChangeDelegate action);
}