using System;
using System.Collections.Generic;
using UnityEngine;

public enum ChasingType {
    Zombifiable,
    Zombie
}

public class Chaser<T> : IChaser<T> where T : MonoBehaviour, IChaseable {

    private readonly string _zombieTag = "Zombie";
    private readonly string _zombifiableTag = "Zombifiable";
    private readonly string _tag;
    private readonly GameObject _gameObject;

    private float _distanceFromTarget;
    private T _target;

    public Chaser(GameObject gameObject, float distanceFromTarget) {
        _gameObject = gameObject;
        _distanceFromTarget = distanceFromTarget;
        _tag = typeof(T).Name switch {
            nameof(Zombie) => _zombieTag,
            nameof(Zombifiable) => _zombifiableTag,
            _ => "",
        };
    }

    public void SetTarget() {
        float closestDistance = Mathf.Infinity;
        var targets = GameObject.FindGameObjectsWithTag(_tag);
        foreach (var target in targets) {
            var chaseable = target.GetComponent<T>();
            if (chaseable != null && chaseable.IsAvailable()) {
                float distance = Vector3.Distance(_gameObject.transform.position, chaseable.transform.position);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    _target = chaseable;
                }
            }
        }
    }

    public T GetTarget() {
        return _target;
    }

    public TargetDistanceState GetTargetDistanceState() {
        if (_target.IsAvailable()) {
            if (Vector2.Distance(_gameObject.transform.position, _target.transform.position) < _distanceFromTarget) {
                return TargetDistanceState.Reached;
            } else {
                return TargetDistanceState.NotReached;
            }
        } else {
            return TargetDistanceState.NotAvailable;
        }
    }

    public void SetAttackRange(float distance) {
        _distanceFromTarget = distance;
    }
}