using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderController : MonoBehaviour
{

    [SerializeField]
    private float _speed = 0.8f;
    [SerializeField]
    private float _range = 10f;
    [SerializeField]
    private float _maxDistance = 10f;
    [SerializeField]
    private MovementController _movementController;

    private Vector2 _target;


    void Start()
    {
        if (enabled)
        {
            SetNewDestination();
        }
    }

    void Update()
    {
        if (!enabled)
        {
            return;
        }
        if (Vector2.Distance(transform.position, _target) < _range)
        {
            SetNewDestination();
        }
    }

    private void SetNewDestination()
    {
        _target = new Vector2(Random.Range(-_maxDistance, _maxDistance), Random.Range(-_maxDistance, _maxDistance));
        _movementController.Move(_speed, (_target - (Vector2)transform.position).normalized);
    }

    public void Enable()
    {
        if (enabled)
        {
            return;
        }
        enabled = true;
        SetNewDestination();
    }

    public void Disable()
    {
        if (!enabled)
        {
            return;
        }
        enabled = false;
    }
}
