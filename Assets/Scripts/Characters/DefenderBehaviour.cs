using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DefenderState
{
    Idle,
    Wandering,
    Chasing,
    AboutToAttack,
    Attacking,
    AboutToDie,
    Death,
}

public enum DefenderType
{
    Melee, Ranged
}

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(DefenderChaser))]
[RequireComponent(typeof(Zombifiable))]
public abstract class Defender : MonoBehaviour
{
    public DefenderType Type;
    public DefenderData Data;

    public bool IsRanged
    {
        get { return Type == DefenderType.Ranged; }
    }

    [ShowIf("IsRanged")]
    [SerializeField]
    protected Transform ProjectileSpawnPosition;

    abstract public AudioClip AttackSound { get; set; }

    private MovementController _movementController;
    private DefenderChaser _chaser;
    private WanderController _wanderController;
    private Zombifiable _zombifiable;
    private Animator _animator;
    private ZombieBehaviour _currentTarget;
    private Collider2D _collider;
    private readonly List<ZombieBehaviour> _zombiesNearby = new();

    public DefenderState State = DefenderState.Idle;
    protected AnimationHelper _animationHelper;

    public abstract void Attack(ZombieBehaviour target);
    public virtual void Awake()
    {
        InitData();
        SetCollider();
        SetState(DefenderState.Idle);

        _movementController = GetComponent<MovementController>();
        _chaser = GetComponent<DefenderChaser>();
        _wanderController = GetComponent<WanderController>();
        _zombifiable = GetComponent<Zombifiable>();
        _animator = GetComponent<Animator>();
        _animationHelper = new AnimationHelper(_animator);
        _animationHelper.SetAttackSpeed(Data.AttackSpeed);
    }

    private void Start()
    {
        Debug.Log("Is data null? " + (Data == null));
        _chaser.SubscribeToTargetChanges(OnTargetChange);
    }

    public virtual void FixedUpdate()
    {
        CleanZombiesList();
        if (IsSelfZombified())
        {
            return;
        }
        switch (State)
        {
            case DefenderState.Idle:
                HandleIdleState();
                break;
            case DefenderState.Chasing:
                HandleChasingState();
                break;
            case DefenderState.AboutToAttack:
                HandleAboutToAttackState();
                break;
            case DefenderState.AboutToDie:
                HandleDeathState();
                break;
            case DefenderState.Attacking:
                if (_currentTarget == null)
                {
                    SetState(DefenderState.Idle);
                }
                break;
            case DefenderState.Wandering:
                HandleWanderingState();
                break;
            default:
                break;
        }
    }

    private void InitData()
    {
        switch (Type)
        {
            case DefenderType.Melee:
                Data = GameBalancer.Instance.GetMeleeDefenderStats();
                break;
            case DefenderType.Ranged:
                Data = GameBalancer.Instance.GetRangedDefenderStats();
                break;
        }
    }

    private void SetCollider()
    {
        _collider = gameObject.AddComponent<CircleCollider2D>();
        (_collider as CircleCollider2D).radius = 3;
        _collider.isTrigger = true;
        _collider.offset = Vector2.zero;
        int zombieLayerMask = 1 << LayerMask.NameToLayer("Zombie");
        int collidersLayerMask = 1 << LayerMask.NameToLayer("Colliders");
        _collider.excludeLayers = ~zombieLayerMask & ~collidersLayerMask;
        _collider.includeLayers = zombieLayerMask | collidersLayerMask;
        _collider.contactCaptureLayers = zombieLayerMask | collidersLayerMask;
        _collider.callbackLayers = zombieLayerMask | collidersLayerMask;
    }

    public void CleanZombiesList() => _zombiesNearby.RemoveAll(zombie => zombie == null || !zombie.IsAvailable());

    public virtual void StartBattle()
    {
        SetState(DefenderState.Idle);
    }


    private void HandleWanderingState()
    {
        if (IsZombiesNearby())
        {
            SetNewTarget();
        }
    }

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
        if (_chaser.Target != null && _chaser.Target.IsAvailable())
        {
            SetState(DefenderState.Chasing);
            _wanderController.Disable();
        }
        else if (IsZombiesNearby())
        {
            SetNewTarget();
        }
        else
        {
            _movementController.Stop();
            _wanderController.Enable(forceEnable: true);
            SetState(DefenderState.Wandering);
        }
    }

    private bool IsZombiesNearby() => _zombiesNearby.Count > 0;

    private void HandleChasingState()
    {
        if (_currentTarget == null || !_currentTarget.IsAvailable())
        {
            SetState(DefenderState.Idle);
        }
        else
        {
            switch (_chaser.GetTargetDistanceState())
            {
                case TargetDistanceState.Reached:
                    _movementController.FaceTarget(_currentTarget.transform);
                    SetState(DefenderState.AboutToAttack);
                    break;
                case TargetDistanceState.NotReached:
                    _movementController.Move(Data.Speed, _currentTarget.gameObject);
                    _animationHelper.PlayAnimation(AnimationType.Running);
                    break;
                case TargetDistanceState.NotAvailable:
                    SetState(DefenderState.Idle);
                    break;
            }
        }
    }

    private void OnTargetChange(ZombieBehaviour target)
    {
        _currentTarget = target;
        if (_currentTarget != null)
        {
            SetState(DefenderState.Chasing);
        }
        else
        {
            SetState(DefenderState.Idle);
        }
    }

    private void HandleAboutToAttackState()
    {
        if (_chaser.GetTargetDistanceState() != TargetDistanceState.Reached)
        {
            SetState(DefenderState.Idle);
            return;
        }
        _movementController.Stop();
        SetState(DefenderState.Attacking);
        Attack(_currentTarget.GetComponent<ZombieBehaviour>());
        StartCoroutine(HandleAboutToAttackStateCore());
    }

    public IEnumerator HandleAboutToAttackStateCore()
    {

        yield return new WaitForSeconds(1 / Data.AttackSpeed);
        if (State == DefenderState.Attacking)
        {
            SetState(DefenderState.Chasing);
        }
    }



    private void HandleDeathState()
    {
        Die();
        SetState(DefenderState.Death);
    }

    private bool IsSelfZombified()
    {
        return _zombifiable.IsZombified();
    }

    public void Die()
    {
        _animationHelper.PlayAnimation(AnimationType.Death);
        _movementController.Disable();
        Destroy(gameObject, 1f);
    }

    private void SetState(DefenderState state)
    {
        State = state;
    }

    private void SetNewTarget()
    {
        if (!IsZombiesNearby())
        {
            return;
        }
        ZombieBehaviour closestZombie = _zombiesNearby[0];
        float distanceToClosestZombie = Vector2.Distance(transform.position, closestZombie.transform.position);
        foreach (ZombieBehaviour zombie in _zombiesNearby)
        {
            if (zombie.IsAvailable())
            {
                float distanceToZombie = Vector2.Distance(transform.position, zombie.transform.position);
                if (distanceToZombie < distanceToClosestZombie)
                {
                    closestZombie = zombie;
                    distanceToClosestZombie = distanceToZombie;
                }
            }
        }
        _chaser.SetTarget(closestZombie);
    }

    private void OnDestroy()
    {
        _chaser.UnsubscribeFromTargetChanges(OnTargetChange);
        RoundManager.Instance.RemoveDefender(this);
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

    public void Empower() => Data.Empower();
}
