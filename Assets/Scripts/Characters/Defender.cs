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
public abstract class Defender : MonoBehaviour, IChaser<Zombie>
{
    public DefenderData Data;
    public DefenderType Type;

    abstract public AudioClip AttackSound { get; set; }

    private MovementController _movementController;
    private DefenderChaser _chaser;
    private WanderController _wanderController;
    private Zombifiable _zombifiable;
    private Animator _animator;

    protected AnimationHelper _animationHelper;
    private DefenderState _state = DefenderState.Idle;


    public abstract void Attack(Transform target);

    public virtual void Awake()
    {
        _movementController = GetComponent<MovementController>();
        _chaser = GetComponent<DefenderChaser>();
        _wanderController = GetComponent<WanderController>();
        _zombifiable = GetComponent<Zombifiable>();
        _animator = GetComponent<Animator>();

        _animationHelper = new AnimationHelper(_animator);

        SetState(DefenderState.Idle);
    }

    public virtual void Update()
    {
        if (IsSelfZombified())
        {
            return;
        }
        switch (_state)
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
                if (_chaser.Target == null)
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
        _chaser.SetTarget();
        if (_chaser.Target != null)
        {
            SetState(DefenderState.Chasing);
            _wanderController.Disable();
        }
        else
        {
            _animationHelper.PlayAnimation(AnimationType.Idle);
            _movementController.Stop();
            _wanderController.Enable();
            SetState(DefenderState.RoundOver);
        }
    }

    private void HandleChasingState()
    {
        if (_chaser.Target == null)
        {
            SetState(DefenderState.Idle);
        }
        else
        {
            if (_chaser.IsTargetReached())
            {
                SetState(DefenderState.AboutToAttack);
            }
            else
            {
                _movementController.Move(Data.Speed, _chaser.Target.gameObject);
                _animationHelper.PlayAnimation(AnimationType.Running);
            }
        }
    }

    public IEnumerator HandleAboutToAttackState()
    {
        if (_chaser.Target == null || _chaser.Target != null && (!_chaser.IsTargetReached() || !_chaser.Target.IsAvailable()))
        {
            SetState(DefenderState.Idle);
            yield break;
        }
        _movementController.Stop();
        SetState(DefenderState.Attacking);
        Attack(_chaser.Target.transform);
        AudioSource.PlayClipAtPoint(AttackSound, transform.position);
        if (Type == DefenderType.Melee)
        {
            _chaser.Target.TakeDamage(Data.Damage);
        }
        yield return new WaitForSeconds(Data.AttackSpeed);
        SetState(DefenderState.Chasing);
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

    public void SetTarget()
    {
        _chaser.SetTarget();
    }

    public Zombie GetTarget()
    {
        return _chaser.Target;
    }

    public bool IsTargetReached()
    {
        return _chaser.IsTargetReached();
    }

    private void SetState(DefenderState state)
    {
        _state = state;
    }

    private void OnDestroy()
    {
        RoundManager.Instance.RemoveDefender(this);
    }
}
