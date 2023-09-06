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

    private float _distanceFromTarget;
    private float _detectionRangePriorityTarget = 3f;

    private T Target;

    public event IChaser<T>.TragetChangeDelegate OnTargetChange;

    public Chaser(GameObject gameObject, float distanceFromTarget)
    {
        _gameObject = gameObject;
        _distanceFromTarget = distanceFromTarget;
        _tag = typeof(T).Name switch
        {
            nameof(Zombie) => _zombieTag,
            nameof(Zombifiable) => _zombifiableTag,
            _ => "",
        };
    }

    //public T GetTargets()
    //{
    //    var targets = GameObject.FindGameObjectsWithTag(_tag);
    //    if (typeof(T) == typeof(Zombifiable))
    //    { // Prioritize Defenders
    //        foreach (var target in targets)
    //        {
    //            var chaseable = target.GetComponent<Defender>();
    //            if (chaseable != null && chaseable.IsAvailable())
    //            {
    //                float distance = Vector3.Distance(_gameObject.transform.position, chaseable.transform.position);
    //                if (distance < _detectionRangePriorityTarget)
    //                {
    //                    return chaseable;
    //                }
    //            }
    //        }
    //    }
    //}

    public T FindNewTarget()
    {
        float closestDistance = Mathf.Infinity;
        var targets = GameObject.FindGameObjectsWithTag(_tag);
        T newTarget = null;
        foreach (var target in targets)
        {
            var chaseable = target.GetComponent<T>();
            if (chaseable != null && chaseable.IsAvailable())
            {
                if (chaseable.IsPriority())
                {
                    newTarget = chaseable;
                    break;
                }
                float distance = Vector3.Distance(_gameObject.transform.position, chaseable.transform.position);
                if (distance < closestDistance)
                {
                    newTarget = chaseable;
                    closestDistance = distance;
                }
            }
        }
        SetTarget(newTarget);
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

    public void SetTarget(T target)
    {
        Target = target;
        NotifyTargetChange();
    }

    public void SubscribeToTargetChanges(IChaser<T>.TragetChangeDelegate action)
    {
        OnTargetChange += action;
    }

    public void UnsubscribeFromTargetChanges(IChaser<T>.TragetChangeDelegate action)
    {
        OnTargetChange -= action;
    }

    private void NotifyTargetChange() => OnTargetChange?.Invoke(Target);
}
