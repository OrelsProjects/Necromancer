using System;
using System.Collections.Generic;
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
    private T _target;

    public Chaser(GameObject gameObject, float distanceFromTarget)
    {
        _gameObject = gameObject;
        _distanceFromTarget = distanceFromTarget;
        switch (typeof(T).Name)
        {
            case nameof(Zombie):
                _tag = _zombieTag;
                break;
            case nameof(Zombifiable):
                _tag = _zombifiableTag;
                break;
            default:
                _tag = "";
                break;
        }
    }

    public void SetTarget()
    {
        float closestDistance = Mathf.Infinity;
        var targets = GameObject.FindGameObjectsWithTag(_tag);
        foreach (var target in targets)
        {
            var chaseable = target.GetComponent<T>();
            if (chaseable != null && chaseable.IsAvailable())
            {
                float distance = Vector3.Distance(_gameObject.transform.position, chaseable.GetTransform().position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _target = chaseable;
                }
            }
        }
    }

    public T GetTarget()
    {
        return _target;
    }

    public bool IsTargetReached()
    {
        return Vector2.Distance(_gameObject.transform.position, _target.GetTransform().position) < _distanceFromTarget;
    }

    public void SetAttackRange(float distance)
    {
        _distanceFromTarget = distance;
    }
}