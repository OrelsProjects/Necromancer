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

    private readonly float _chanceToScream = 0.05f;

    private MovementController _movementController;
    private WanderController _wanderController;
    private Zombifiable _zombifiable;
    private Collider2D _collider;
    private readonly List<ZombieBehaviour> _zombiesNearby = new();


    public CivilianState _state = CivilianState.Idle;

    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
        _wanderController = GetComponent<WanderController>();
        _zombifiable = GetComponent<Zombifiable>();
        SetCollider();
    }

    private void Update()
    {
        RemoveDeadZombiesFromList();
        if (_zombifiable.IsZombified())
        {
            SetState(CivilianState.Dead);
            _movementController.Disable(true);
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
            case CivilianState.Wander:
                HandleWanderState();
                break;
        }
    }

    private void SetCollider()
    {
        _collider = gameObject.AddComponent<CircleCollider2D>();
        _collider.isTrigger = true;
        (_collider as CircleCollider2D).radius = 1f;
        _collider.offset = Vector2.zero;
        int zombieLayerMask = 1 << LayerMask.NameToLayer("Zombie");
        _collider.excludeLayers = ~zombieLayerMask;
        _collider.includeLayers = zombieLayerMask;
        _collider.contactCaptureLayers = zombieLayerMask;
        _collider.callbackLayers = zombieLayerMask;
    }

    private void RemoveDeadZombiesFromList() => _zombiesNearby.RemoveAll(zombie => zombie == null || !zombie.IsAvailable());

    private void HandleIdleState()
    {
        bool isRoundStarted = RoundManager.Instance.IsRoundStarted;
        if (_collider != null)
        {
            _collider.enabled = isRoundStarted;
        }
        if (!isRoundStarted)
        {
            return;
        }
        if (!SetShouldRunState())
        {
            _wanderController.Enable();
            SetState(CivilianState.Wander);
        }
    }

    private void HandleWanderState()
    {
        SetShouldRunState();
    }

    private bool SetShouldRunState()
    {
        bool isZombieNearby = IsZombieNearby();
        if (isZombieNearby)
        {
            SetState(CivilianState.AboutToRun);
        }
        return isZombieNearby;
    }

    private void HandleAboutToRunState()
    {
        SetState(CivilianState.Running);
        _wanderController.Disable();
        RunAwayFromZombie();
        StartCoroutine(DelayChangingTargets());
        Scream();
    }

    private void Scream()
    {
        if (Random.Range(0f, 1f) <= _chanceToScream)
        {
            AudioSource.PlayClipAtPoint(_screamSound, transform.position, 0.2f);
        }
    }

    private void RunAwayFromZombie()
    {
        Vector3? zombiePosition;
        if (_zombiesNearby.Count <= 0)
        {
            SetState(CivilianState.Idle);
            return;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            _zombiesNearby.Add(collision.gameObject.GetComponent<ZombieBehaviour>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            _zombiesNearby.Remove(collision.gameObject.GetComponent<ZombieBehaviour>());
        }
    }

}