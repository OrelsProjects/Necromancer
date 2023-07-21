using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombieState
{
    Idle,
    Chasing,
    AboutToAttack,
    Attacking,
    Death,
    RoundOver,
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Animator))]
public class Zombie : MonoBehaviour, IChaseable
{

    [SerializeField]
    private float _speed = 2.5f;
    [SerializeField]
    private float _attackSpeed = 1f;
    [SerializeField]
    private float _health = 100f;

    private Rigidbody2D _rigidbody;
    private MovementController _movementController;
    private Animator _animator;
    private ZombieChaser _chaser;

    private WanderController _wanderController;
    private AnimationHelper _animationHelper;

    private ZombieState _state = ZombieState.Idle;


    private void Awake()
    {
        gameObject.tag = "Zombie";
    }


    private void Start()
    {
        RoundManager.Instance.AddZombie(this);

        _rigidbody = GetComponent<Rigidbody2D>();
        _movementController = GetComponent<MovementController>();
        _animator = GetComponent<Animator>();
        _chaser = GetComponent<ZombieChaser>();
        _wanderController = GetComponent<WanderController>();

        _animationHelper = new AnimationHelper(_animator);
    }

    private void Update()
    {
        if (_health <= 0)
        {
            _state = ZombieState.Death;
            Destroy(gameObject, 1f);
        }
        if (_chaser.Target == null && _state != ZombieState.RoundOver)
        {
            _state = ZombieState.Idle;
            _animationHelper.Idle();
        }
        switch (_state)
        {
            case ZombieState.Idle:
                _movementController.Stop();
                _chaser.SetTarget();
                if (_chaser.Target != null)
                {
                    _state = ZombieState.Chasing;
                }
                else
                {
                    FinishRound();
                }
                break;

            case ZombieState.Chasing:
                Chase();
                break;
            case ZombieState.AboutToAttack:
                StartCoroutine(zombifyTarget());
                break;
            case ZombieState.Death:
                _movementController.Stop();
                _animationHelper.Death();
                break;
            default: break;
        }
    }

    private void Chase()
    {
        if (_chaser.IsTargetReached())
        {
            _movementController.Move(0, _chaser.Target.gameObject);
            _state = ZombieState.AboutToAttack;
        }
        else
        {
            _movementController.Move(_speed, _chaser.Target.gameObject);
        }
    }

    private void FinishRound()
    {
        _wanderController.Enable();
        _state = ZombieState.RoundOver;
    }

    private IEnumerator zombifyTarget()
    {
        if (_chaser.Target == null || _chaser.Target.IsZombified())
        {
            _state = ZombieState.Idle;
            yield break;
        }
        _state = ZombieState.Attacking;
        if (_chaser.Target != null)
        {
            _animationHelper.AttackMelee();
            _chaser.Target.Zombify();
            yield return new WaitForSeconds(1 / _attackSpeed);
            _state = ZombieState.Chasing;
        }
    }

    public void TakeDamage(float damage)
    {
        _animationHelper.Hit();
        _health -= damage;
    }

    public void Heal(float health)
    {
        _animationHelper.Heal();
        _health += health;
    }

    public bool IsAlive()
    {
        return _health > 0;
    }

    public bool IsAvailable()
    {
        return _health > 0;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void OnDestroy()
    {
        RoundManager.Instance.RemoveZombie(this);
    }
}