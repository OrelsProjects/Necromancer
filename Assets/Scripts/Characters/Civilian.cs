using System.Collections;
using UnityEngine;

public enum CivilianState
{
    Idle,
    AboutToRun,
    Running,
    Dead,
}

[RequireComponent(typeof(MovementController))]
public class Civilian : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private float _maxDistanceFromZombie = 4f;
    [SerializeField]
    private float _delayBetweenChangingTargets = 2f;

    private MovementController _movementController;
    private WanderController _wanderController;
    private Zombifiable _zombifiable;
    private CivilianState _state = CivilianState.Idle;

    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
        _wanderController = GetComponent<WanderController>();
        _zombifiable = GetComponent<Zombifiable>();
    }

    private void Update()
    {
        if (_zombifiable.IsZombified())
        {
            SetState(CivilianState.Dead);
            return;
        }
        switch (_state)
        {
            case CivilianState.Idle:
                HandleIdleState();
                break;
            case CivilianState.AboutToRun:
                HandleAboutToRunState();
                break;
        }
    }

    private void HandleIdleState()
    {
        if (IsZombieNearby())
        {
            SetState(CivilianState.AboutToRun);
        }
        else
        {
            _wanderController.Enable();
        }
    }

    private void HandleAboutToRunState()
    {
        if (IsZombieNearby())
        {
            _wanderController.Disable();
            RunAwayFromZombie();
            SetState(CivilianState.Running);
            StartCoroutine(DelayChangingTargets());
        }
        else
        {
            SetState(CivilianState.Idle);
        }
    }

    private void RunAwayFromZombie()
    {
        Vector3? zombiePosition = RoundManager.Instance.GetClosestZombiePosition(transform.position);
        if (zombiePosition == null)
        {
            return;
        }
        Vector3 direction = (transform.position - zombiePosition.Value).normalized;
        _movementController.Move(_speed, direction);
    }

    private bool IsZombieNearby()
    {
        Vector3? closestZombie = RoundManager.Instance.GetClosestZombiePosition(transform.position);
        if (closestZombie == null)
        {
            return false;
        }
        return Vector3.Distance(transform.position, closestZombie.Value) < _maxDistanceFromZombie;
    }

    private IEnumerator DelayChangingTargets()
    {
        yield return new WaitForSeconds(_delayBetweenChangingTargets);
        SetState(CivilianState.Idle);
    }

    private void SetState(CivilianState state)
    {
        _state = state;
    }
}