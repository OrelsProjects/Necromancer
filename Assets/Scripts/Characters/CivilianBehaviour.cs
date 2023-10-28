using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CivilianState
{
    Idle,
    Wander,
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
    private float _delayBetweenChangingTargets = 2f;

    [Header("Sound")]
    [SerializeField]
    private AudioClip _screamSound;

    private readonly float _chanceToScream = 0.3f;

    private MovementController _movementController;
    private WanderController _wanderController;
    private Zombifiable _zombifiable;
    private CivilianState _state = CivilianState.Idle;
    private readonly List<ZombieBehaviour> _zombiesNearby = new();

    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
        _wanderController = GetComponent<WanderController>();
        _zombifiable = GetComponent<Zombifiable>();
        SetCollider();
    }

    private void Update()
    {
        CleanZombiesList();
        if (_zombifiable.IsZombified())
        {
            SetState(CivilianState.Dead);
            _movementController.Disable(true);
            return;
        }
        if (IsZombieNearby())
        {
            SetState(CivilianState.AboutToRun);
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

    private void SetCollider()
    {
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 1;
        collider.offset = Vector2.zero;
        collider.includeLayers = LayerMask.GetMask("Zombie");
        collider.excludeLayers = -1;
    }

    private void CleanZombiesList() => _zombiesNearby.RemoveAll(zombie => zombie == null || !zombie.IsAvailable());

    private void HandleIdleState()
    {
        if (IsZombieNearby())
        {
            SetState(CivilianState.AboutToRun);
        }
        else
        {
            _wanderController.Enable();
            SetState(CivilianState.Wander);
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
            Scream();
        }
        else
        {
            SetState(CivilianState.Idle);
        }
    }

    private void Scream()
    {
        if (Random.Range(0f, 1f) < _chanceToScream)
        {
            AudioSource.PlayClipAtPoint(_screamSound, transform.position);
        }
    }

    private void RunAwayFromZombie()
    {
        Vector3? zombiePosition;
        zombiePosition = _zombiesNearby[0].transform.position;

        if (zombiePosition == null)
        {
            return;
        }

        Vector3 randomDirectionBias = Random.insideUnitCircle;
        Vector3 direction = transform.position - zombiePosition.Value + randomDirectionBias;
        Vector3 directionNormalized = direction.normalized;
        _movementController.Move(_speed, directionNormalized);
        StartCoroutine(ChangeDirection());
    }

    private IEnumerator ChangeDirection()
    {
        yield return new WaitForSeconds(3);
        SetState(CivilianState.AboutToRun);
    }

    private bool IsZombieNearby() => _zombiesNearby.Count > 0;

    private IEnumerator DelayChangingTargets()
    {
        yield return new WaitForSeconds(_delayBetweenChangingTargets);
        SetState(CivilianState.Idle);
    }

    private void SetState(CivilianState state)
    {
        _state = state;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Zombie"))
        {
            _zombiesNearby.Add(other.gameObject.GetComponent<ZombieBehaviour>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Zombie"))
        {
            _zombiesNearby.Remove(other.gameObject.GetComponent<ZombieBehaviour>());
        }
    }

}