using System.Collections;
using UnityEngine;

public enum DefenderState
{
    Idle,
    Chasing,
    AboutToAttack,
    Attacking,
    Death,
    RoundOver,
}

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(DefenderChaser))]
[RequireComponent(typeof(Zombifiable))]
public abstract class Defender : MonoBehaviour, IChaser<Zombie>
{
    public int Health { get; set; }
    public int Damage { get; set; }
    public float Speed { get; set; }
    public float AttackSpeed { get; set; }

    private MovementController _movementController;
    private DefenderChaser _chaser;
    private WanderController _wanderController;
    private Animator _animator;

    protected  AnimationHelper _animationHelper;
    private DefenderState _state = DefenderState.Idle;

    public abstract void Attack();

    public virtual void Start()
    {
        RoundManager.Instance.AddDefender(this);

        _chaser = GetComponent<DefenderChaser>();
        _animator = GetComponent<Animator>();
        _movementController = GetComponent<MovementController>();
        _animationHelper = new AnimationHelper(_animator);
        _wanderController = GetComponent<WanderController>();
    }

    public virtual void Update()
    {
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
            case DefenderState.Death:
                HandleDeathState();
                break;
            default:
                break;
        }
    }

    private void HandleIdleState()
    {
        _chaser.SetTarget();
        if (_chaser.Target != null)
        {
            _state = DefenderState.Chasing;
            _wanderController.Disable();
        }
    }

    private void HandleChasingState()
    {
        if (_chaser.Target == null)
        {
            _state = DefenderState.Idle;
            _wanderController.Enable();
        }
        else
        {
            if (_chaser.IsTargetReached())
            {
                _state = DefenderState.AboutToAttack;
            }
            else
            {
                _movementController.Move(Speed, _chaser.Target.transform.position);
            }
        }
    }

    public IEnumerator HandleAboutToAttackState()
    {
        _state = DefenderState.Attacking;
        Attack();
        _chaser.Target.TakeDamage(Damage);
        yield return new WaitForSeconds(AttackSpeed);
        _state = DefenderState.Chasing;
    }

    private void HandleDeathState()
    {
        Die();
    }

    public void TakeDamage(int damage)
    {
        Health = Mathf.Max(Health - damage, 0);
        if (Health <= 0 && _state != DefenderState.Death)
        {
            _state = DefenderState.Death;
        }
    }

    public void Die()
    {
        _animationHelper.Death();
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
}
