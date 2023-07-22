using System.Collections;
using UnityEngine;

public enum ZombieState
{
    Idle,
    Chasing,
    AboutToAttack,
    Attacking,
    AboutToDie,
    Dead,
    RoundOver,
}

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Animator))]
public class Zombie : MonoBehaviour, IChaseable
{
    [SerializeField] private float _speed = 2.5f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private float _health = 100f;

    private MovementController _movementController;
    private Animator _animator;
    private ZombieChaser _chaser;
    private WanderController _wanderController;
    private AnimationHelper _animationHelper;
    private ZombieState _state = ZombieState.Idle;

    private void Awake()
    {
        tag = "Zombie";
        _movementController = GetComponent<MovementController>();
        _animator = GetComponent<Animator>();
        _chaser = GetComponent<ZombieChaser>();
        _wanderController = GetComponent<WanderController>();
        _animationHelper = new AnimationHelper(_animator);
    }

    private void Start()
    {
        RoundManager.Instance.AddZombie(this);
    }

    private void OnDestroy()
    {
        RoundManager.Instance.RemoveZombie(this);
    }

    private void Update()
    {
        if (_chaser.Target == null && _state != ZombieState.RoundOver)
        {
            SetState(ZombieState.Idle);
            _animationHelper.PlayAnimation(AnimationType.Idle);
        }

        switch (_state)
        {
            case ZombieState.Idle:
                HandleIdleState();
                break;
            case ZombieState.Chasing:
                Chase();
                break;
            case ZombieState.AboutToAttack:
                StartCoroutine(ZombifyTarget());
                break;
            case ZombieState.AboutToDie:
                HandleDeath();
                break;
        }
    }

    private void HandleIdleState()
    {
        _movementController.Stop();
        _chaser.SetTarget();
        if (_chaser.Target != null)
        {
            SetState(ZombieState.Chasing);
        }
        else
        {
            FinishRound();
        }
    }

    private void Chase()
    {
        if (_chaser.IsTargetReached())
        {
            _movementController.Move(0, _chaser.Target.gameObject);
            SetState(ZombieState.AboutToAttack);
        }
        else
        {
            _movementController.Move(_speed, _chaser.Target.gameObject);
            _animationHelper.PlayAnimation(AnimationType.Running);
        }
    }

    private void FinishRound()
    {
        _wanderController.Enable();
        SetState(ZombieState.RoundOver);
    }

    private IEnumerator ZombifyTarget()
    {
        if (!_chaser.Target.IsAvailable() || _chaser.Target.IsZombified())
        {
            SetState(ZombieState.Idle);
            yield break;
        }

        SetState(ZombieState.Attacking);
        _animationHelper.PlayAnimation(AnimationType.AttackMelee);
        _chaser.Target.Zombify();
        yield return new WaitForSeconds(1 / _attackSpeed);

        if (_state == ZombieState.Attacking)
        {
            SetState(ZombieState.Chasing);
        }
    }

    private void HandleDeath()
    {
        SetState(ZombieState.Dead);
        _movementController.Stop();
        _wanderController.Disable();
        _animationHelper.PlayAnimation(AnimationType.Death);
        Destroy(gameObject, 1f);
    }

    private void SetState(ZombieState state)
    {
        _state = state;
    }

    public void TakeDamage(float damage)
    {
        _animationHelper.PlayAnimation(AnimationType.Hit);
        _health -= damage;
        if (_health <= 0 && _state != ZombieState.AboutToDie)
        {
            SetState(ZombieState.AboutToDie);
        }
    }

    public void Heal(float health)
    {
        _animationHelper.PlayAnimation(AnimationType.Heal);
        _health += health;
    }

    public bool IsAlive() => _health > 0;

    public bool IsAvailable() => _health > 0;

    public Transform GetTransform() => transform;
}
