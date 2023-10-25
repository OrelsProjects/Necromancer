using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DefenderState
{
    Idle,
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
    public DefenderData Data;
    public DefenderType Type;

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
    private Zombie _currentTarget;
    private readonly List<Zombie> _zombiesNearby = new();

    protected DefenderState State = DefenderState.Idle;
    protected AnimationHelper _animationHelper;

    public abstract void Attack(Zombie target);

    public virtual void Awake()
    {
        _movementController = GetComponent<MovementController>();
        _chaser = GetComponent<DefenderChaser>();
        _wanderController = GetComponent<WanderController>();
        _zombifiable = GetComponent<Zombifiable>();
        _animator = GetComponent<Animator>();

        _animationHelper = new AnimationHelper(_animator);
        _animationHelper.SetAttackSpeed(Data.AttackSpeed);

        SetCollider();
        SetState(DefenderState.Idle);
    }

    private void Start()
    {
        _chaser.SubscribeToTargetChanges(OnTargetChange);
    }

    public virtual void Update()
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
                StartCoroutine(HandleAboutToAttackState());
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
            default:
                break;
        }
    }

    private void SetCollider()
    {
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 2;
        collider.offset = Vector2.zero;
    }

    public void CleanZombiesList() => _zombiesNearby.RemoveAll(zombie => zombie == null || !zombie.IsAvailable());


    public virtual void StartBattle()
    {
        SetState(DefenderState.Idle);
    }

    private void HandleIdleState()
    {
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
            _animationHelper.PlayAnimation(AnimationType.Idle);
            _wanderController.Enable();
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

    private void OnTargetChange(Zombie target)
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

    public IEnumerator HandleAboutToAttackState()
    {
        if (_chaser.GetTargetDistanceState() != TargetDistanceState.Reached)
        {
            SetState(DefenderState.Idle);
            yield break;
        }
        _movementController.Stop();
        SetState(DefenderState.Attacking);
        Attack(_currentTarget.GetComponent<Zombie>());
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
        Zombie closestZombie = _zombiesNearby[0];
        float distanceToClosestZombie = Vector2.Distance(transform.position, closestZombie.transform.position);
        foreach (Zombie zombie in _zombiesNearby)
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
            _zombiesNearby.Add(collision.gameObject.GetComponent<Zombie>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            _zombiesNearby.Remove(collision.gameObject.GetComponent<Zombie>());
        }
    }
}
