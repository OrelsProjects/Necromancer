using UnityEngine;

public enum ChasingType
{
    Zombifiable,
    Zombie
}


public class Chaser<T> : IChaser<T> where T : MonoBehaviour, IChaseable
{

    private readonly string _zombieTag = "Zombie";
    private readonly string _zombifiableTag = "Zombifiable";
    private readonly string _tag;
    private readonly GameObject _gameObject;

    // Owner is the only one allowed to set the target.
    private Owner _owner;
    private float _distanceFromTarget;
    private readonly float _detectionRangePriorityTarget = 3f;

    private T Target;

    public event IChaser<T>.TargetChangeDelegate OnTargetChange;

    private bool IsOwner(Owner potentialOwner) => potentialOwner.gameObject != null && _owner.gameObject != null && potentialOwner.gameObject.GetInstanceID() == _owner.gameObject.GetInstanceID();

    public Chaser(GameObject gameObject, float distanceFromTarget, Owner owner)
    {
        _gameObject = gameObject;
        _distanceFromTarget = distanceFromTarget;
        _tag = typeof(T).Name switch
        {
            nameof(ZombieBehaviour) => _zombieTag,
            nameof(Zombifiable) => _zombifiableTag,
            _ => "",
        };
        SetOwner(owner);
    }

    public T FindNewTarget()
    {
        float closestDistance = Mathf.Infinity;
        bool isPriorityTargetsOnly = false; // After the first priority target found, only priority targets will be considered.
        GameObject[] targets = GameObject.FindGameObjectsWithTag(_tag);
        T newTarget = null;
        foreach (var target in targets)
        {
            var chaseable = target.GetComponent<T>();
            if (chaseable != null && chaseable.IsAvailable())
            {
                if (isPriorityTargetsOnly && !chaseable.IsPriority())
                {
                    continue;
                }
                float distance = Vector3.Distance(_gameObject.transform.position, chaseable.transform.position);
                if (!isPriorityTargetsOnly && chaseable.IsPriority())
                {
                    isPriorityTargetsOnly = true;
                    newTarget = chaseable;
                    closestDistance = distance;
                    continue;
                }
                if (distance < closestDistance)
                {
                    newTarget = chaseable;
                    closestDistance = distance;
                }
            }
        }
        SetTarget(newTarget, _owner);
        return Target;
    }

    public T GetTarget()
    {
        return Target;
    }

    public TargetDistanceState GetTargetDistanceState()
    {
        if (Target.IsAvailable())
        {
            if (Vector2.Distance(_gameObject.transform.position, Target.transform.position) < _distanceFromTarget)
            {
                return TargetDistanceState.Reached;
            }
            else
            {
                return TargetDistanceState.NotReached;
            }
        }
        else
        {
            return TargetDistanceState.NotAvailable;
        }
    }

    public void SetAttackRange(float distance)
    {
        _distanceFromTarget = distance;
    }

    public void SetTarget(T target, Owner owner)
    {
        if (IsOwner(owner))
        {
            Target = target;
            NotifyTargetChange();
        }
    }

    public void SubscribeToTargetChanges(IChaser<T>.TargetChangeDelegate action)
    {
        OnTargetChange += action;
    }

    public void UnsubscribeFromTargetChanges(IChaser<T>.TargetChangeDelegate action)
    {
        OnTargetChange -= action;
    }

    private void NotifyTargetChange() => OnTargetChange?.Invoke(Target);

    public void SetOwner(Owner owner)
    {
        if (_owner.priority <= owner.priority)
        {
            _owner = owner;
        }
        else
        {
            Debug.LogWarning("Owner must have a higher priority.");
        }
    }
}
