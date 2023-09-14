using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public enum DefenderState
{
    Idle,
    Chasing,
    AboutToAttack,
    Attacking,
    AboutToDie,
    Death,
    RoundOver,
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

        SetState(DefenderState.Idle);
    }

    private void Start()
    {
        _chaser.SubscribeToTargetChanges(OnTargetChange);
    }

    public virtual void Update()
    {
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
        else if (!_chaser.FindNewTarget())
        {
            _animationHelper.PlayAnimation(AnimationType.Idle);
            _movementController.Stop();
            _wanderController.Enable();
            if (!ZombieSpawner.Instance.AreThereZombiesToSpawn())
            {
                SetState(DefenderState.RoundOver);
            }
        }
    }

    private void HandleChasingState()
    {
        if (_currentTarget == null)
        {
            SetState(DefenderState.Idle);
        }
        else
        {
            if (_chaser.GetTargetDistanceState() == TargetDistanceState.Reached)
            {
                _movementController.FaceTarget(_currentTarget.transform);
                SetState(DefenderState.AboutToAttack);
            }
            else
            {
                _movementController.Move(Data.Speed, _currentTarget.gameObject);
                _animationHelper.PlayAnimation(AnimationType.Running);
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

    private void OnDestroy()
    {
        _chaser.UnsubscribeFromTargetChanges(OnTargetChange);
        RoundManager.Instance.RemoveDefender(this);
    }
}
